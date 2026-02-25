using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;

namespace OrderManagement.Provisioner.Api.Configuration
{
    public class ValidateAddressAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAddressRepository _addressRepository;
        private readonly IMemoryCache _cache;

        public ValidateAddressAuthorizationFilter(
            ICurrentUserProvider currentUserProvider,
            IAddressRepository addressRepository,
            IMemoryCache cache)
        {
            _currentUserProvider = currentUserProvider;
            _addressRepository = addressRepository;
            _cache = cache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var value) || value is not int id)
            {
                context.Result = new BadRequestResult();
                return;
            }

            var cacheKey = $"address-auth:{id}";

            var cachedUserId = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                var address = await _addressRepository.GetAsync(id, context.HttpContext.RequestAborted);
                return address?.UserId;
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