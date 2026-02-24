using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;

namespace OrderManagement.Provisioner.Api.Configuration
{
    public class ValidateAddressAuthorizationFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAddressRepository _addressRepository;

        public ValidateAddressAuthorizationFilter(
            ICurrentUserProvider currentUserProvider,
            IAddressRepository addressRepository)
        {
            _currentUserProvider = currentUserProvider;
            _addressRepository = addressRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var value) || value is not int id)
            {
                context.Result = new ForbidResult();
                return;
            }

            var address = await _addressRepository.GetAsync(id, context.HttpContext.RequestAborted);

            if (address is null || address.UserId != _currentUserProvider.GetLoggedUserId())
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}