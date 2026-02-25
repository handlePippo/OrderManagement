using OrderManagement.Gateway.Application.Interfaces;

namespace OrderManagement.Gateway.Configuration
{
    public sealed class AuthorizationForwardingHandler : DelegatingHandler
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        private const string AuthHeadeUserId = "X-User-Id";
        private const string AuthHeaderUserType = "X-User-Type";

        private const string AdminRole = "Admin";
        private const string UserRole = "User";

        public AuthorizationForwardingHandler(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Forward(request, AuthHeadeUserId, _currentUserProvider.GetLoggedUserId().ToString());
            Forward(request, AuthHeaderUserType, _currentUserProvider.IsAdmin ? AdminRole : UserRole);

            return base.SendAsync(request, cancellationToken);
        }

        private static void Forward(HttpRequestMessage request, string name, string value)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            request.Headers.Remove(name);
            request.Headers.TryAddWithoutValidation(name, value);
        }
    }
}