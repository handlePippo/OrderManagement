using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;

namespace OrderManagement.Provisioner.Api.Configuration
{
    public class ValidateUserAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;

        public ValidateUserAuthorizationFilter(ICurrentUserProvider currentUserProvider,
            IUserRepository userRepository,
            IMemoryCache cache)
        {
            _currentUserProvider = currentUserProvider;
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var value) || value is not int id)
            {
                context.Result = new ForbidResult();
                return;
            }

            var cacheKey = $"user-auth:{id}";

            var cachedUserId = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                var user = await _userRepository.GetAsync(id, context.HttpContext.RequestAborted);
                return user?.Id;
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