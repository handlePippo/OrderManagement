namespace OrderManagement.Order.Api.Configuration
{
    public sealed class JwtForwardingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authHeader = _httpContextAccessor
                .HttpContext?
                .Request
                .Headers
                .Authorization
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}