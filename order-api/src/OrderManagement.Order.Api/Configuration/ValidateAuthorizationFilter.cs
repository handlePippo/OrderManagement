using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;

namespace OrderManagement.Order.Api.Configuration
{
    public class ValidateAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOrderRepository _orderRepository;
        private readonly IMemoryCache _cache;

        public ValidateAuthorizationFilter(
            ICurrentUserProvider currentUserProvider,
            IOrderRepository orderRepository,
            IMemoryCache cache)
        {
            _currentUserProvider = currentUserProvider;
            _orderRepository = orderRepository;
            _cache = cache;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var value) || value is not Guid id)
            {
                context.Result = new BadRequestResult();
                return;
            }

            var cacheKey = $"order-auth:{id}";

            var cachedUserId = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                var order = await _orderRepository.GetAsync(id, context.HttpContext.RequestAborted);
                return order?.UserId;
            });

            if (cachedUserId is null || (cachedUserId.Value != _currentUserProvider.GetLoggedUserId() && !_currentUserProvider.IsAdmin))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}