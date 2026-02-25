using AutoFixture;
using FluentAssertions;
using OrderManagement.Gateway.Persistence.Extensions;
using System.Security.Claims;

namespace OrderManagement.Gateway.Infrastructure.Tests.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void IsAdmin_WhenUserIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal user = null!;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => user.IsAdmin());
            ex.ParamName.Should().Be("user");
        }

        [Fact]
        public void IsAdmin_WhenRoleIsAdmin_ReturnsTrue()
        {
            // Arrange
            var user = CreateUser(role: "Admin", nameIdentifier: _fixture.Create<int>().ToString());

            // Act
            var result = user.IsAdmin();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsAdmin_WhenRoleIsNotAdmin_ReturnsFalse()
        {
            // Arrange
            var user = CreateUser(role: "User", nameIdentifier: _fixture.Create<int>().ToString());

            // Act
            var result = user.IsAdmin();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsAdmin_WhenRoleClaimIsMissing_ReturnsFalse()
        {
            // Arrange
            var user = CreateUser(role: null, nameIdentifier: _fixture.Create<int>().ToString());

            // Act
            var result = user.IsAdmin();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetLoggedUserId_WhenUserIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal user = null!;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => user.GetLoggedUserId());
            ex.ParamName.Should().Be("user");
        }

        [Fact]
        public void GetLoggedUserId_WhenNameIdentifierIsValidInt_ReturnsParsedInt()
        {
            // Arrange
            var expectedId = _fixture.Create<int>();
            var user = CreateUser(role: "User", nameIdentifier: expectedId.ToString());

            // Act
            var id = user.GetLoggedUserId();

            // Assert
            id.Should().Be(expectedId);
        }

        [Fact]
        public void GetLoggedUserId_WhenNameIdentifierIsMissing_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser(role: "User", nameIdentifier: null);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => user.GetLoggedUserId());
            ex.Message.Should().Be("Invalid sub claim");
        }

        [Fact]
        public void GetLoggedUserId_WhenNameIdentifierIsNotInt_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = CreateUser(role: "User", nameIdentifier: _fixture.Create<string>());

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => user.GetLoggedUserId());
            ex.Message.Should().Be("Invalid sub claim");
        }

        private static ClaimsPrincipal CreateUser(string? role, string? nameIdentifier)
        {
            // Arrange
            var claims = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (nameIdentifier is not null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
            }

            var identity = new ClaimsIdentity(claims, authenticationType: "Test");
            return new ClaimsPrincipal(identity);
        }
    }
}