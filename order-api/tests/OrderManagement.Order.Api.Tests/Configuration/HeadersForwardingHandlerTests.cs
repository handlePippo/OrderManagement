using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using OrderManagement.Order.Api.Configuration;
using System.Net;

namespace OrderManagement.Order.Api.Tests.Configuration
{
    public sealed class HeadersForwardingHandlerTests
    {
        private readonly CapturingHandler _innerHandler = new();
        private readonly TestableAuthorizationForwardingHandler _sut;
        private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        public HeadersForwardingHandlerTests()
        {
            _sut = new TestableAuthorizationForwardingHandler(_httpContextAccessor)
            {
                InnerHandler = _innerHandler
            };
        }

        [Fact]
        public async Task SendAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            var act = () => _sut.SendAsyncPublic(null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SendAsync_WhenHttpContextIsNull_DoesNotForwardHeaders()
        {
            // Arrange
            _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test");
            request.Headers.TryAddWithoutValidation("X-User-Id", "existing");
            request.Headers.TryAddWithoutValidation("X-User-Type", "existingRole");

            // Act
            var _ = await _sut.SendAsyncPublic(request, default);

            // Assert
            request.Should().NotBeNull();

            request!.Headers.TryGetValues("X-User-Id", out var userIdValues).Should().BeTrue();
            userIdValues!.Single().Should().Be("existing");

            request.Headers.TryGetValues("X-User-Type", out var roleValues).Should().BeTrue();
            roleValues!.Single().Should().Be("existingRole");
        }

        [Fact]
        public async Task SendAsync_WhenHeadersExistInHttpContext_ForwardsBothHeaders()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-User-Id"] = "123";
            httpContext.Request.Headers["X-User-Type"] = "Admin";

            _httpContextAccessor.HttpContext.Returns(httpContext);

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test");

            // Act
            var _ = await _sut.SendAsyncPublic(request, default);

            // Assert
            request.Should().NotBeNull();

            request!.Headers.TryGetValues("X-User-Id", out var userIdValues).Should().BeTrue();
            userIdValues!.Single().Should().Be("123");

            request.Headers.TryGetValues("X-User-Type", out var roleValues).Should().BeTrue();
            roleValues!.Single().Should().Be("Admin");
        }

        [Fact]
        public async Task SendAsync_WhenRequestAlreadyHasHeaders_ReplacesWithContextValues()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-User-Id"] = "new-user";
            httpContext.Request.Headers["X-User-Type"] = "new-role";

            _httpContextAccessor.HttpContext.Returns(httpContext);

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test");
            request.Headers.TryAddWithoutValidation("X-User-Id", "old-user");
            request.Headers.TryAddWithoutValidation("X-User-Type", "old-role");

            // Act
            var _ = await _sut.SendAsyncPublic(request, default);

            // Assert
            request!.Headers.GetValues("X-User-Id").Single().Should().Be("new-user");
            request.Headers.GetValues("X-User-Type").Single().Should().Be("new-role");
        }

        [Fact]
        public async Task SendAsync_WhenOnlyOneHeaderExists_ForwardsOnlyThatHeader()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-User-Id"] = "123";

            _httpContextAccessor.HttpContext.Returns(httpContext);


            using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test");

            // Act
            var _ = await _sut.SendAsyncPublic(request, default);

            // Assert
            request.Headers.Contains("X-User-Id").Should().BeTrue();
            request.Headers.GetValues("X-User-Id").Single().Should().Be("123");
            request.Headers.Contains("X-User-Type").Should().BeFalse();
        }

        private sealed class TestableAuthorizationForwardingHandler : HeadersForwardingHandler
        {
            public TestableAuthorizationForwardingHandler(IHttpContextAccessor ctx) : base(ctx) { }

            public Task<HttpResponseMessage> SendAsyncPublic(HttpRequestMessage request, CancellationToken cancellationToken) => SendAsync(request, cancellationToken);
        }

        private sealed class CapturingHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}