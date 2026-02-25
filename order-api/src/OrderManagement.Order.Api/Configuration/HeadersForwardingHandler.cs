namespace OrderManagement.Order.Api.Configuration
{
    public sealed class HeadersForwardingHandler : DelegatingHandler
    {
        private const string AuthHeadeUserId = "X-User-Id";
        private const string AuthHeaderUserType = "X-User-Type";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeadersForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            Forward(context, request, AuthHeadeUserId);
            Forward(context, request, AuthHeaderUserType);

            return base.SendAsync(request, cancellationToken);
        }

        private static void Forward(HttpContext ctx, HttpRequestMessage request, string name)
        {
            ArgumentNullException.ThrowIfNull(ctx);
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (ctx.Request.Headers.TryGetValue(name, out var value))
            {
                request.Headers.Remove(name);
                request.Headers.TryAddWithoutValidation(name, value.ToString());
            }
        }
    }
}