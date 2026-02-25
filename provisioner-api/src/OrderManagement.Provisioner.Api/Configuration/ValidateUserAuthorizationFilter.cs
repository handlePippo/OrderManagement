using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;

namespace OrderManagement.Provisioner.Api.Configuration
{
    public class ValidateUserAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserRepository _userRepository;

        public ValidateUserAuthorizationFilter(
            ICurrentUserProvider currentUserProvider,
            IUserRepository userRepository)
        {
            _currentUserProvider = currentUserProvider;
            _userRepository = userRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var value) || value is not int id)
            {
                context.Result = new ForbidResult();
                return;
            }

            var user = await _userRepository.GetAsync(id, context.HttpContext.RequestAborted);

            if (user is null || (user.Id != _currentUserProvider.GetLoggedUserId() && !_currentUserProvider.IsAdmin))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}