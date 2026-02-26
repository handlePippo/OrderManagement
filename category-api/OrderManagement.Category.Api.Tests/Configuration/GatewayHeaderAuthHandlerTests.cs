using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using OrderManagement.Category.Api.Configuration;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace OrderManagement.Category.Api.Tests.Configuration
{
    public sealed class GatewayHeaderAuthHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GatewayHeaderAuthHandler _sut;
        private readonly IOptionsMonitor<AuthenticationSchemeOptions> _optionsMonitor = Substitute.For<IOptionsMonitor<AuthenticationSchemeOptions>>();
        private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();
        private readonly UrlEncoder _urlEncoder = Substitute.For<UrlEncoder>();

        public GatewayHeaderAuthHandlerTests()
        {
            _optionsMonitor.Get(Arg.Any<string>()).Returns(new AuthenticationSchemeOptions());

            _fixture.Inject(_optionsMonitor);
            _fixture.Inject(_loggerFactory);
            _fixture.Inject(_urlEncoder);

            _sut = _fixture.Create<GatewayHeaderAuthHandler>();
        }

        [Fact]
        public async Task AuthenticateAsync_WhenUserIdHeaderIsMissing_ReturnsNoResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            await InitializeHandlerAsync(httpContext);

            // Act
            var result = await _sut.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
            result.None.Should().BeTrue();
            result.Failure.Should().BeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_WhenUserIdHeaderIsWhitespace_ReturnsNoResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-User-Id"] = "   ";

            await InitializeHandlerAsync(httpContext);

            // Act
            var result = await _sut.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
            result.None.Should().BeTrue();
        }

        [Fact]
        public async Task AuthenticateAsync_WhenUserIdHeaderExists_ReturnsSuccessWithExpectedClaims()
        {
            // Arrange
            var userId = _fixture.Create<string>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-User-Id"] = userId;

            await InitializeHandlerAsync(httpContext);

            // Act
            var result = await _sut.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Ticket.Should().NotBeNull();
            result.Ticket!.AuthenticationScheme.Should().Be(GatewayHeaderAuthHandler.SchemeName);

            var principal = result.Principal;
            principal.Should().NotBeNull();

            principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be(userId);
            principal.FindFirst("sub")!.Value.Should().Be(userId);

            // Nessun ruolo se header X-User-Type mancante
            principal.FindFirst(ClaimTypes.Role).Should().BeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_WhenRoleHeaderExists_AddsRoleClaim()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var role = _fixture.Create<string>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-User-Id"] = userId;
            httpContext.Request.Headers["X-User-Type"] = role;

            await InitializeHandlerAsync(httpContext);

            // Act
            var result = await _sut.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeTrue();

            var principal = result.Principal!;
            principal.FindFirst(ClaimTypes.Role)!.Value.Should().Be(role);
        }

        private async Task InitializeHandlerAsync(HttpContext httpContext)
        {
            var scheme = new AuthenticationScheme(GatewayHeaderAuthHandler.SchemeName, GatewayHeaderAuthHandler.SchemeName, typeof(GatewayHeaderAuthHandler));
            await _sut.InitializeAsync(scheme, httpContext);
        }
    }
}