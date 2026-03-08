using AutoMapper;
using OrderManagement.Order.Api.Application.Bags;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.Extensions;
using OrderManagement.Order.Api.Application.Factories;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Services
{
    public sealed partial class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderNormalizerService _normalizer;
        private readonly IProductApiClient _productApiClient;
        private readonly IProvisionerApiClient _provisionerApiClient;
        private int CurrentUserId => _currentUserProvider.GetLoggedUserId();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="repository"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="orderItemRepository"></param>
        /// <param name="productApiClient"></param>
        /// <param name="provisionerApiClient"></param>
        /// <param name="productCalculationsNormalizer"></param>
        public OrderService(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IOrderRepository repository,
            IUnitOfWork unitOfWork,
            IOrderItemRepository orderItemRepository,
            IProductApiClient productApiClient,
            IProvisionerApiClient provisionerApiClient,
            IOrderNormalizerService productCalculationsNormalizer)
        {
            _mapper = mapper;
            _orderRepository = repository;
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
            _productApiClient = productApiClient;
            _provisionerApiClient = provisionerApiClient;
            _normalizer = productCalculationsNormalizer;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) => await _orderRepository.ExistsAsync(id, cancellationToken);

        public async Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.ListAsync(cancellationToken);
            if (orders is null)
            {
                return Array.Empty<OrderDto>()!;
            }

            var orderItemsIds = orders
                .Select(o => o.Id)
                .ToList()
                .AsReadOnly();

            var orderItems = await _orderItemRepository.GetRangeByOrderIdAsync(orderItemsIds, cancellationToken);
            if (orderItems is null)
            {
                return Array.Empty<OrderDto>()!;
            }

            var lookup = orderItems.ToLookup(x => x.OrderId);
            foreach (var order in orders)
            {
                order.SetItems(lookup[order.Id].ToList());
            }

            return _mapper.Map<IReadOnlyList<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(id, cancellationToken);
            if (order is null)
            {
                return null;
            }

            var orderItems = await _orderItemRepository.GetRangeByOrderIdAsync(id, cancellationToken);
            if (orderItems is null)
            {
                return null;
            }

            order.SetItems(orderItems);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task CreateAsync(CreateOrderDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var normalizedOrderTask = NormalizeOrderAsync(dto, token);
            var shippingAddressTask = GetShippingAddressAsync(dto.AddressId, token);

            await Task.WhenAll(normalizedOrderTask, shippingAddressTask);

            var normalizedOrder = normalizedOrderTask.GetAwaiter().GetResult();
            var shippingAddress = shippingAddressTask.GetAwaiter().GetResult();

            var order = OrderFactory.Create(CurrentUserId, shippingAddress, normalizedOrder.SubTotal, normalizedOrder.Total);
            var orderItems = _normalizer.NormalizeOrderItems(order, normalizedOrder.Products, normalizedOrder.OrderItemsToBeProcessed);

            await ExecuteCoordinatedCreateAsync(order, orderItems, normalizedOrder.ProductStock, token);
        }

        public async Task UpdateAsync(Guid id, UpdateOrderDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var order = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(order);

            if (dto.Items is not null && dto.Items.Count == 0)
            {
                throw new InvalidOperationException("Items list cannot be empty.");
            }

            var updateBag = await UpdateAsync(dto, order, token);

            if (updateBag.ExecuteUpdate)
            {
                await ExecuteCoordinatedUpdateAsync(order, updateBag, token);
            }
        }

        public async Task SubmitAsync(Guid id, CancellationToken token)
        {
            var order = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(order);

            order.SetStatus(OrderStatus.Submitted);
            order.MarkModified();

            await _orderRepository.UpdateAsync(order, token);
        }

        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var dbOrder = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(dbOrder);

            var stock = await GetOldProductStockToUpdateAsync(dbOrder, token);

            await ExecuteCoordinatedDeleteAsync(id, stock, token);
        }

        public async Task DeleteSubmittedAsync(Guid id, CancellationToken token)
        {
            var order = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            if (order.Status != OrderStatus.Submitted)
            {
                throw new InvalidOperationException("The order is not in submitted status.");
            }

            if (DateTime.Now - order.CreatedAt > TimeSpan.FromHours(24))
            {
                throw new InvalidOperationException("It's not possibile to delete a submitted order after 24 hours.");
            }

            order.SetStatus(OrderStatus.Deleted);
            order.MarkModified();

            await _orderRepository.UpdateAsync(order, token);

            var stock = await GetOldProductStockToUpdateAsync(order, token);
            await _productApiClient.IncreaseStock(stock, token);
        }

        #region UoW handlers

        private async Task ExecuteCoordinatedCreateAsync(Domain.Entities.Order order, IReadOnlyList<OrderItem> orderItemsToBeUpdated, ProductStock stockUpdate, CancellationToken token)
        {
            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                await _orderItemRepository.AddRangeAsync(orderItemsToBeUpdated, token);
                await _orderRepository.AddAsync(order, token);

                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitAsync(token);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(token);
                throw;
            }

            await _productApiClient.DecreaseStock(stockUpdate, token);
        }

        private async Task ExecuteCoordinatedUpdateAsync(Domain.Entities.Order order, UpdateBag bag, CancellationToken token)
        {
            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                if (bag.OrderItemsToBeUpdated?.Count > 0)
                {
                    await _orderItemRepository.DeleteRangeByOrderIdAsync(order.Id, token);
                    await _orderItemRepository.AddRangeAsync(bag.OrderItemsToBeUpdated, token);
                }

                await _orderRepository.UpdateAsync(order, token);

                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitAsync(token);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(token);
                throw;
            }

            if (bag.OrderItemsToBeUpdated?.Count > 0)
            {
                var increaseTask = _productApiClient.IncreaseStock(bag.OldStockToUpdate, token);
                var decreaseTask = _productApiClient.DecreaseStock(bag.NewStockToUpdate, token);
                await Task.WhenAll(increaseTask, decreaseTask);
            }
        }

        private async Task ExecuteCoordinatedDeleteAsync(Guid orderId, ProductStock stockUpdate, CancellationToken token)
        {
            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                await _orderItemRepository.DeleteRangeByOrderIdAsync(orderId, token);
                await _orderRepository.DeleteAsync(orderId, token);

                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitAsync(token);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(token);
                throw;
            }

            await _productApiClient.IncreaseStock(stockUpdate, token);
        }

        #endregion

        #region Private Methods

        private async Task<UpdateBag> UpdateAsync(UpdateOrderDto dto, Domain.Entities.Order order, CancellationToken token)
        {
            IReadOnlyList<OrderItem> orderItemsToBeUpdated = [];
            var executeUpdate = false;

            if (dto.Items?.Count > 0)
            {
                var normalizedOrder = await NormalizeOrderAsync(dto, token);
                order.ApplyPatchFrom(normalizedOrder.SubTotal, normalizedOrder.Total);
                orderItemsToBeUpdated = _normalizer.NormalizeOrderItems(order, normalizedOrder.Products, normalizedOrder.OrderItemsToBeProcessed, true);
                executeUpdate = true;
            }

            if (dto.AddressId is int addressId && addressId > 0)
            {
                var shippingAddress = await GetShippingAddressAsync(addressId, token);
                order.ApplyPatchFrom(shippingAddress);
                executeUpdate = true;
            }

            ProductStock oldStockToUpdate = null!;
            ProductStock newStockToUpdate = null!;
            if (executeUpdate)
            {
                var stockTask = GetOldProductStockToUpdateAsync(order, token);
                var oldStockTask = GetNewProductStockToUpdateAsync(orderItemsToBeUpdated, token);

                await Task.WhenAll(stockTask, oldStockTask);

                oldStockToUpdate = stockTask.GetAwaiter().GetResult();
                newStockToUpdate = oldStockTask.GetAwaiter().GetResult();
            }

            return new UpdateBag() with
            {
                OrderItemsToBeUpdated = orderItemsToBeUpdated,
                OldStockToUpdate = oldStockToUpdate,
                NewStockToUpdate = newStockToUpdate,
                ExecuteUpdate = executeUpdate
            };
        }


        private async Task<ProductStock> GetOldProductStockToUpdateAsync(Domain.Entities.Order order, CancellationToken token)
        {
            var orderItems = await _orderItemRepository.GetRangeByOrderIdAsync(order.Id, token);
            var productsToUpdate = orderItems.ToDictionary(o => o.ProductId, b => b.Quantity);
            var products = await _productApiClient.GetProductsAsync(new ProductRange([.. productsToUpdate.Keys]), token);

            var stock = new ProductStock();
            foreach (var product in products)
            {
                stock.Lines.Add(new ProductStockLine
                {
                    ProductId = product.Id,
                    Quantity = productsToUpdate[product.Id]
                });
            }

            return stock;
        }

        private async Task<ProductStock> GetNewProductStockToUpdateAsync(IReadOnlyList<OrderItem> orderItems, CancellationToken token)
        {
            var productsToUpdate = orderItems.ToDictionary(o => o.ProductId, b => b.Quantity);
            var products = await _productApiClient.GetProductsAsync(new ProductRange([.. productsToUpdate.Keys]), token);

            var stock = new ProductStock();
            foreach (var product in products)
            {
                stock.Lines.Add(new ProductStockLine
                {
                    ProductId = product.Id,
                    Quantity = productsToUpdate[product.Id]
                });
            }

            return stock;
        }

        private async Task<NormalizedOrderBag> NormalizeOrderAsync(CreateOrderDto requestDto, CancellationToken token)
            => await NormalizeAsync(requestDto.Items, token);

        private async Task<NormalizedOrderBag> NormalizeOrderAsync(UpdateOrderDto requestDto, CancellationToken token)
            => await NormalizeAsync(requestDto.Items!, token);

        private async Task<NormalizedOrderBag> NormalizeAsync(IReadOnlyList<OrderItemProductInfoDto> requestDto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(requestDto);

            var bag = GetCompositeItems(requestDto);

            var products = await _productApiClient.GetProductsAsync(new ProductRange(bag.ProductIds), token);

            var (subTotal, total, oldProductStockToUpdate) = _normalizer.NormalizeOrderItemsCalculations(products, bag.OrderItems);

            return new NormalizedOrderBag() with
            {
                SubTotal = subTotal,
                Total = total,
                Products = products,
                OrderItemsToBeProcessed = bag.OrderItemsByIdAndQty,
                ProductStock = oldProductStockToUpdate
            };
        }

        private async Task<ShippingAddress> GetShippingAddressAsync(int addressId, CancellationToken token)
        {
            var shippingAddressTask = _provisionerApiClient.GetShippingAddressAsync(addressId, token);
            var userTask = _provisionerApiClient.GetUserAsync(CurrentUserId, token);

            await Task.WhenAll(shippingAddressTask, userTask);

            var shippingAddress = shippingAddressTask.GetAwaiter().GetResult();
            var user = userTask.GetAwaiter().GetResult();

            shippingAddress.ShipPhoneNumber = user.PhoneNumber;

            return shippingAddress;
        }

        private static CompositeBag GetCompositeItems(IReadOnlyList<OrderItemProductInfoDto> requestDto)
        {
            var orderItems = requestDto
                                .Select(i => new OrderItem(i.ProductId, i.Quantity))
                                .ToList()
                                .AsReadOnly();

            var orderItemsByIdAndQty = orderItems
                                        .GroupBy(i => i.ProductId)
                                        .ToDictionary(g => g.Key, g => g
                                        .Sum(x => x.Quantity));

            var productsIds = orderItemsByIdAndQty
                                .Keys
                                .ToList()
                                .AsReadOnly();

            return new CompositeBag() with
            {
                OrderItems = orderItems,
                OrderItemsByIdAndQty = orderItemsByIdAndQty,
                ProductIds = productsIds
            };
        }

        private static void ValidateStatus(Domain.Entities.Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Is not possible to edit an order that is not in pending status.");
            }
        }

        #endregion
    }
}