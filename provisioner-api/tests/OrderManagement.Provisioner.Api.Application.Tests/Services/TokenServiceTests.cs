using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Application.Services;
using OrderManagement.Provisioner.Api.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;

namespace OrderManagement.Provisioner.Api.Application.Tests.Services
{
    public sealed class TokenServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly TokenService _sut;
        private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        private const string JwtHexKey = "00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF";
        private const string JwtIssuer = "test-issuer";

        public TokenServiceTests()
        {
            _configuration["Jwt:Key"].Returns(JwtHexKey);
            _configuration["Jwt:Issuer"].Returns(JwtIssuer);

            _fixture.Inject(_configuration);
            _fixture.Inject(_userRepository);
            _fixture.Inject(_mapper);

            _sut = _fixture.Create<TokenService>();
        }

        [Fact]
        public async Task GetTokenAsync_WhenInvalidCredentials_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = _fixture.Create<TokenRequestDto>();
            var request = _fixture.Create<TokenRequest>();

            _mapper.Map<TokenRequest>(dto).Returns(request);
            _userRepository.GetAsync(request, default).Returns((Domain.Entities.User?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetTokenAsync(dto, default));
            ex.Message.Should().Be("Invalid credentials.");

            _mapper.Received(1).Map<TokenRequest>(dto);
            await _userRepository.Received(1).GetAsync(request, default);
        }

        [Fact]
        public async Task GetTokenAsync_WhenAdminEmail_SetsAdminRoleAndSubClaim()
        {
            // Arrange
            var userId = _fixture.Create<int>();

            var dto = _fixture.Build<TokenRequestDto>()
                .With(x => x.Email, "admin@demo.it")
                .Create();

            var request = _fixture.Build<TokenRequest>()
                .With(x => x.Email, "admin@demo.it")
                .Create();

            var user = new Domain.Entities.User(userId);

            _mapper.Map<TokenRequest>(dto).Returns(request);
            _userRepository.GetAsync(request, default).Returns(user);

            // Act
            var result = await _sut.GetTokenAsync(dto, default);

            // Assert
            result.Should().NotBeNull();
            result.Jwt.Should().NotBeNullOrWhiteSpace();

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Jwt);

            jwt.Issuer.Should().Be(JwtIssuer);
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
            jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");

            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow.AddHours(23));
            jwt.ValidTo.Should().BeBefore(DateTime.UtcNow.AddHours(25));
        }

        [Fact]
        public async Task GetTokenAsync_WhenNonAdminEmail_SetsUserRole()
        {
            // Arrange
            var userId = _fixture.Create<int>();

            var dto = _fixture.Build<TokenRequestDto>()
                .With(x => x.Email, "user@demo.it")
                .Create();

            var request = _fixture.Build<TokenRequest>()
                .With(x => x.Email, "user@demo.it")
                .Create();

            var user = new Domain.Entities.User(userId);

            _mapper.Map<TokenRequest>(dto).Returns(request);
            _userRepository.GetAsync(request, default).Returns(user);

            // Act
            var result = await _sut.GetTokenAsync(dto, default);

            // Assert
            result.Jwt.Should().NotBeNullOrWhiteSpace();

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Jwt);

            jwt.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "User");
        }

        [Fact]
        public async Task GetTokenAsync_UsesRepositoryAndMapper()
        {
            // Arrange
            var dto = _fixture.Create<TokenRequestDto>();
            var request = _fixture.Create<TokenRequest>();
            var user = _fixture.Create<Domain.Entities.User>();

            _mapper.Map<TokenRequest>(dto).Returns(request);
            _userRepository.GetAsync(request, default).Returns(user);

            // Act
            _ = await _sut.GetTokenAsync(dto, default);

            // Assert
            _mapper.Received(1).Map<TokenRequest>(dto);
            await _userRepository.Received(1).GetAsync(request, default);
        }
    }
}