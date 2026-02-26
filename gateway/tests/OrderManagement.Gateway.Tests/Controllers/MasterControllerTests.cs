using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Token;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using OrderManagement.Gateway.Controllers;

namespace OrderManagement.Gateway.Tests.Controllers
{
    public class MasterControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly MasterController _sut;

        private readonly IProvisionerApiMasterClient _client = Substitute.For<IProvisionerApiMasterClient>();

        public MasterControllerTests()
        {
            _sut = new MasterController(_client);
        }

        [Fact]
        public async Task Login_WhenClientReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<TokenRequestDto>();

            _client.GetTokenAsync(request, token).Returns((TokenResponseDto?)null!);

            // Act
            var result = await _sut.Login(request, token);

            // Assert
            result.Result.Should().BeOfType<BadRequestResult>();

            await _client.Received(1).GetTokenAsync(request, token);
        }

        [Fact]
        public async Task Login_WhenClientReturnsToken_ReturnsOkWithToken()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<TokenRequestDto>();
            var jwt = _fixture.Create<TokenResponseDto>();

            _client.GetTokenAsync(request, token).Returns(jwt);

            // Act
            var result = await _sut.Login(request, token);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(jwt);

            await _client.Received(1).GetTokenAsync(request, token);
        }

        [Fact]
        public async Task CreateAsync_CallsClientAndReturnsCreated()
        {
            // Arrange
            var token = _fixture.Create<CancellationToken>();
            var request = _fixture.Create<CreateUserDto>();

            // Act
            var result = await _sut.CreateAsync(request, token);

            // Assert
            result.Result.Should().BeOfType<CreatedResult>();

            await _client.Received(1).CreateUserAsync(request, token);
        }
    }
}