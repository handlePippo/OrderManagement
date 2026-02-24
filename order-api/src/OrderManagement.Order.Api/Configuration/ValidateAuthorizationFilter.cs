using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;

namespace OrderManagement.Order.Api.Configuration
{
    public class ValidateAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOrderRepository _orderRepository;

        public ValidateAuthorizationFilter(
            ICurrentUserProvider currentUserProvider,
            IOrderRepository orderRepository)
        {
            _currentUserProvider = currentUserProvider;
            _orderRepository = orderRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var value) || value is not int id)
            {
                context.Result = new ForbidResult();
                return;
            }

            var order = await _orderRepository.GetAsync(id, context.HttpContext.RequestAborted);

            if (order is null || order.UserId != _currentUserProvider.GetLoggedUserId())
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}