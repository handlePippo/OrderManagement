using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;

namespace OrderManagement.Gateway.Infrastructure.Tests
{
    public class CurrentUserProviderTests
    {
        private readonly Fixture _fixture = new();
        private readonly IHttpContextAccessor _http = Substitute.For<IHttpContextAccessor>();
        private readonly CurrentUserProvider _sut;

        public CurrentUserProviderTests()
        {
            _fixture.Inject(_http);
            _sut = _fixture.Create<CurrentUserProvider>();
        }

        [Fact]
        public void IsAdmin_WhenHttpContextIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            _http.HttpContext.Returns((HttpContext?)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _ = _sut.IsAdmin);
            ex.Message.Should().Be("Invalid or missing User in HttpContext.");
        }

        [Fact]
        public void GetLoggedUserId_WhenHttpContextIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            _http.HttpContext.Returns((HttpContext?)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _sut.GetLoggedUserId());
            ex.Message.Should().Be("Invalid or missing User in HttpContext.");
        }

        [Fact]
        public void IsAdmin_WhenUserHasAdminRole_ReturnsTrue()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var principal = CreatePrincipal(userId, isAdmin: true);

            var context = new DefaultHttpContext { User = principal };
            _http.HttpContext.Returns(context);

            // Act
            var result = _sut.IsAdmin;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsAdmin_WhenUserIsNotAdmin_ReturnsFalse()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var principal = CreatePrincipal(userId, isAdmin: false);

            var context = new DefaultHttpContext { User = principal };
            _http.HttpContext.Returns(context);

            // Act
            var result = _sut.IsAdmin;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetLoggedUserId_WhenUserHasIdClaim_ReturnsUserId()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var principal = CreatePrincipal(userId, isAdmin: _fixture.Create<bool>());

            var context = new DefaultHttpContext { User = principal };
            _http.HttpContext.Returns(context);

            // Act
            var result = _sut.GetLoggedUserId();

            // Assert
            result.Should().Be(userId);
        }

        private static ClaimsPrincipal CreatePrincipal(int userId, bool isAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("sub", userId.ToString()),
                new Claim("userId", userId.ToString()),
                new Claim("uid", userId.ToString()),
                new Claim("id", userId.ToString()),
            };

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                claims.Add(new Claim("role", "Admin"));
                claims.Add(new Claim("roles", "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                claims.Add(new Claim("role", "User"));
                claims.Add(new Claim("roles", "User"));
            }

            var identity = new ClaimsIdentity(claims, authenticationType: "Test");
            return new ClaimsPrincipal(identity);
        }
    }
}