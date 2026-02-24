using AutoMapper;
using OrderManagement.Order.Api.Application.DTOs.Orders;
using OrderManagement.Order.Api.Application.DTOs.Orders.Create;
using OrderManagement.Order.Api.Application.DTOs.Orders.Update;
using OrderManagement.Order.Api.Application.Extensions;
using OrderManagement.Order.Api.Application.Factories;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductCalculationsNormalizer _normalizer;
        private readonly IProductApiClient _productApiClient;
        private int CurrentUserId => _currentUserProvider.GetLoggedUserId();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="repository"></param>
        /// <param name="productApiClient"></param>
        /// <param name="productCalculationsNormalizer"></param>
        public OrderService(
            IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IOrderRepository repository,
            IUnitOfWork unitOfWork,
            IOrderItemRepository orderItemRepository,
            IProductApiClient productApiClient,
            IProductCalculationsNormalizer productCalculationsNormalizer)
        {
            _mapper = mapper;
            _orderRepository = repository;
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
            _productApiClient = productApiClient;
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

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _orderRepository.ExistsAsync(id, cancellationToken);
        }

        public async Task<OrderDto?> GetAsync(int id, CancellationToken cancellationToken)
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

            var productsInfo = _mapper.Map<IReadOnlyList<OrderItemProductInfo>>(dto.Items);
            var shippingAddress = _mapper.Map<ShippingAddress>(dto.ShippingAddress);

            var orderInfo = await GetTotals(productsInfo, token);
            var order = OrderFactory.Create(CurrentUserId, shippingAddress, orderInfo.SubTotal, orderInfo.Total);

            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                var orderId = await _orderRepository.AddAsync(order, token);

                var orderItemsProductInfo = _normalizer.NormalizeProductsInfo(orderInfo.Products, orderInfo.ItemsToBeAdded);
                var orderItems = OrderItemFactory.Create(orderId, orderItemsProductInfo);

                await _orderItemRepository.AddRangeAsync(orderItems, token);
                await _unitOfWork.SaveChangesAsync(token);
                await _unitOfWork.CommitAsync(token);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(token);
                throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken token = default)
        {
            var dbOrder = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(dbOrder);

            await _orderRepository.DeleteAsync(id, token);
            await _unitOfWork.SaveChangesAsync(token);
        }

        public async Task UpdateAsync(int id, UpdateOrderDto dto, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var order = await _orderRepository.GetAsync(id, token)
                ?? throw new InvalidOperationException("The requested order does not exists.");

            ValidateStatus(order);

            await using var tx = await _unitOfWork.BeginTransactionAsync(token);
            try
            {
                IReadOnlyList<OrderItem>? orderItems = null;

                if (dto.Items is not null && dto.Items.Count == 0)
                {
                    throw new ValidationException("Items cannot be empty.");
                }
                else if (dto.Items is not null)
                {
                    var productsInfo = _mapper.Map<IReadOnlyList<OrderItemProductInfo>>(dto.Items);

                    var orderInfo = await GetTotals(productsInfo, token);

                    order.SetTotals(orderInfo.SubTotal, orderInfo.Total);
                    order.MarkModified();

                    var orderItemsProductInfo = _normalizer.NormalizeProductsInfo(orderInfo.Products, orderInfo.ItemsToBeAdded);
                    orderItems = OrderItemFactory.Create(id, orderItemsProductInfo);
                }

                order.ApplyPatchFrom(dto, order.ShippingAddress);

                await _orderRepository.UpdateAsync(order, token);

                if (orderItems?.Count > 0)
                {
                    await _orderItemRepository.DeleteByOrderIdAsync(id, token);
                    await _orderItemRepository.AddRangeAsync(orderItems, token);
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

        private async Task<OrderInfo> GetTotals(IReadOnlyList<OrderItemProductInfo> productsInfo, CancellationToken token)
        {
            var (itemsToBeAdded, idsOfItemsToBeAdded) = GetCompositeItems(productsInfo);

            var products = await _productApiClient.GetRangeAsync(idsOfItemsToBeAdded, token)
                ?? throw new InvalidOperationException("Failed to recover products.");

            var (subTotal, total) = _normalizer.NormalizeProductsCalculations(products, productsInfo);

            return new OrderInfo(subTotal, total, products, itemsToBeAdded);
        }

        private static void ValidateStatus(Domain.Entities.Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Is not possible to edit an order that is not in pending status.");
            }
        }

        private static (Dictionary<int, int>, IReadOnlyList<int>) GetCompositeItems(IReadOnlyList<OrderItemProductInfo> productsInfo)
        {
            var itemsToBeUpdated = productsInfo!
                                    .GroupBy(i => i.ProductId)
                                    .ToDictionary(g => g.Key, g => g
                                    .Sum(x => x.Quantity));

            var idsOfItemsToUpdated = itemsToBeUpdated
                                                    .Keys
                                                    .ToList()
                                                    .AsReadOnly();

            return (itemsToBeUpdated, idsOfItemsToUpdated);
        }

        private sealed record OrderInfo(decimal SubTotal, decimal Total, IReadOnlyList<Product> Products, Dictionary<int, int> ItemsToBeAdded);

        #endregion
    }
}