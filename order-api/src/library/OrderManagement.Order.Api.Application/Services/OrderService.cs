using AutoMapper;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.Extensions;
using OrderManagement.Order.Api.Application.Factories;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;

namespace OrderManagement.Order.Api.Application.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderNormalizer _normalizer;
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
            IOrderNormalizer productCalculationsNormalizer)
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

        public async Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken)
        {
            var users = await _orderRepository.ListAsync(cancellationToken);
            if (users is null)
            {
                return null!;
            }

            return _mapper.Map<IReadOnlyList<OrderDto>>(users);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _orderRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<OrderDto?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _orderRepository.GetAsync(id, cancellationToken);
            if (user is null)
            {
                return null;
            }

            return _mapper.Map<OrderDto>(user);
        }


        public async Task CreateAsync(CreateOrderDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                // Order
                var normalizedOrder = await NormalizeOrderAsync(dto, token);
                var order = OrderFactory.Create(CurrentUserId, normalizedOrder.ShippingAddress!, normalizedOrder.SubTotal, normalizedOrder.Total);
                await _orderRepository.AddAsync(order, token);

                // OrderItems
                var normalizedOrderItems = _normalizer.NormalizeOrderItems(order, normalizedOrder.Products, normalizedOrder.OrderItemsToBeProcessed);
                await _orderItemRepository.AddRangeAsync(normalizedOrderItems, token);

                // Save and commit
                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitAsync(token);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(token);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var dbOrder = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(dbOrder);

            await _orderRepository.DeleteAsync(id, token);
            await _unitOfWork.SaveChangesAsync(token);
        }

        public async Task UpdateAsync(Guid id, UpdateOrderDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var order = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(order);

            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                IReadOnlyList<OrderItem>? orderItemsToBeUpdated = null;

                if (dto.Items?.Count == 0)
                {
                    throw new InvalidOperationException("Items list cannot be empty.");
                }
                else if (dto.Items?.Count > 0)
                {
                    var normalizedOrder = await NormalizeOrderAsync(dto, token);
                    order.ApplyPatchFrom(normalizedOrder.SubTotal, normalizedOrder.Total, normalizedOrder.ShippingAddress ?? order.ShippingAddress);

                    orderItemsToBeUpdated = _normalizer.NormalizeOrderItems(order, normalizedOrder.Products, normalizedOrder.OrderItemsToBeProcessed, true);
                }
                
                await _orderItemRepository.DeleteByOrderIdAsync(id, token);
                await _orderRepository.UpdateAsync(order, token);

                if (orderItemsToBeUpdated?.Count > 0)
                {
                    await _orderItemRepository.DeleteByOrderIdAsync(id, token);
                    await _orderItemRepository.AddRangeAsync(orderItemsToBeUpdated, token);
                }

                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitAsync(token);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(token);
                throw;
            }
        }

        #region Private Methods

        private async Task<NormalizedOrder> NormalizeOrderAsync(CreateOrderDto requestDto, CancellationToken token)
            => await NormalizeAsync(requestDto.Items, requestDto.AddressId, token);

        private async Task<NormalizedOrder> NormalizeOrderAsync(UpdateOrderDto requestDto, CancellationToken token)
            => await NormalizeAsync(requestDto.Items!, requestDto.AddressId, token);

        private async Task<NormalizedOrder> NormalizeAsync(IReadOnlyList<OrderItemProductInfoDto> requestDto, int? shippingAddressId, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(requestDto);

            var bag = GetCompositeItems(requestDto);

            var products = await _productApiClient.GetProductsAsync(new ProductRange(bag.ProductIds), token);

            var (subTotal, total) = _normalizer.NormalizeOrderItemsCalculations(products, bag.OrderItems);

            ShippingAddress? shippingAddress = null;
            if (shippingAddressId is int addressId)
            {
                shippingAddress = await _provisionerApiClient.GetShippingAddressAsync(addressId, token);
                var user = await _provisionerApiClient.GetUserAsync(CurrentUserId, token);
                shippingAddress.ShipPhoneNumber = user.PhoneNumber;
            }

            return new NormalizedOrder(subTotal, total, shippingAddress, products, bag.OrderItemsByIdAndQty);
        }

        private static void ValidateStatus(Domain.Entities.Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Is not possible to edit an order that is not in pending status.");
            }
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

            return new CompositeBag(orderItems, orderItemsByIdAndQty, productsIds);
        }

        private readonly record struct NormalizedOrder(decimal SubTotal, decimal Total, ShippingAddress? ShippingAddress, IReadOnlyList<Product> Products, Dictionary<int, int> OrderItemsToBeProcessed);
        private readonly record struct CompositeBag(IReadOnlyList<OrderItem> OrderItems, Dictionary<int, int> OrderItemsByIdAndQty, IReadOnlyList<int> ProductIds);

        #endregion
    }
}