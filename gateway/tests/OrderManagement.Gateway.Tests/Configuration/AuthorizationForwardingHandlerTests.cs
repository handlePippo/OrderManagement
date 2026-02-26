using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Gateway.Application.Interfaces;
using OrderManagement.Gateway.Configuration;
using System.Net;

namespace OrderManagement.Gateway.Tests.Configuration
{
    public class AuthorizationForwardingHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();

        private readonly CapturingHandler _innerHandler = new();
        private readonly TestableAuthorizationForwardingHandler _sut;

        public AuthorizationForwardingHandlerTests()
        {
            _sut = new TestableAuthorizationForwardingHandler(_currentUserProvider)
            {
                InnerHandler = _innerHandler
            };
        }

        [Fact]
        public async Task SendAsync_WhenUserIsAdmin_ForwardsUserIdAndAdminUserType()
        {
            // Arrange
            var userId = _fixture.Create<int>();

            _currentUserProvider.GetLoggedUserId().Returns(userId);
            _currentUserProvider.IsAdmin.Returns(true);

            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/api");

            // Act
            var response = await _sut.SendAsyncPublic(request, default);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _innerHandler.LastRequest.Should().NotBeNull();
            _innerHandler.LastRequest!.Headers.TryGetValues("X-User-Id", out var idValues).Should().BeTrue();
            idValues!.Single().Should().Be(userId.ToString());

            _innerHandler.LastRequest.Headers.TryGetValues("X-User-Type", out var typeValues).Should().BeTrue();
            typeValues!.Single().Should().Be("Admin");
        }

        [Fact]
        public async Task SendAsync_WhenUserIsNotAdmin_ForwardsUserIdAndUserUserType()
        {
            // Arrange
            var userId = _fixture.Create<int>();

            _currentUserProvider.GetLoggedUserId().Returns(userId);
            _currentUserProvider.IsAdmin.Returns(false);

            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/api");

            // Act
            var response = await _sut.SendAsyncPublic(request, default);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _innerHandler.LastRequest!.Headers.TryGetValues("X-User-Id", out var idValues).Should().BeTrue();
            idValues!.Single().Should().Be(userId.ToString());

            _innerHandler.LastRequest.Headers.TryGetValues("X-User-Type", out var typeValues).Should().BeTrue();
            typeValues!.Single().Should().Be("User");
        }

        [Fact]
        public async Task SendAsync_WhenHeadersAlreadyExist_ReplacesTheirValues()
        {
            // Arrange
            var userId = _fixture.Create<int>();

            _currentUserProvider.GetLoggedUserId().Returns(userId);
            _currentUserProvider.IsAdmin.Returns(true);

            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/api");
            request.Headers.TryAddWithoutValidation("X-User-Id", "old-value");
            request.Headers.TryAddWithoutValidation("X-User-Type", "old-type");

            // Act
            await _sut.SendAsyncPublic(request, default);

            // Assert
            request.Headers.TryGetValues("X-User-Id", out var idValues).Should().BeTrue();
            idValues!.Should().ContainSingle().Which.Should().Be(userId.ToString());

            request.Headers.TryGetValues("X-User-Type", out var typeValues).Should().BeTrue();
            typeValues!.Should().ContainSingle().Which.Should().Be("Admin");
        }

        [Fact]
        public async Task SendAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            // Act
            var act = () => _sut.SendAsyncPublic(null!, default);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        private sealed class TestableAuthorizationForwardingHandler : AuthorizationForwardingHandler
        {
            public TestableAuthorizationForwardingHandler(ICurrentUserProvider currentUserProvider)
                : base(currentUserProvider)
            {
            }

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