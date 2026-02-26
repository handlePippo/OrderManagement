using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.DTOs.Token;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Controllers;

namespace OrderManagement.Provisioner.Api.Tests.Controllers
{
    public class MasterControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly TokenController _sut;
        private readonly ITokenService _service = Substitute.For<ITokenService>();

        public MasterControllerTests()
        {
            _sut = new TokenController(_service);
        }

        [Fact]
        public async Task Login_WhenClientReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var request = _fixture.Create<TokenRequestDto>();

            _service.GetTokenAsync(request, default).Returns((TokenResponseDto?)null!);

            // Act
            var result = await _sut.Login(request, default);

            // Assert
            result.Result.Should().BeOfType<BadRequestResult>();

            await _service.Received(1).GetTokenAsync(request, default);
        }

        [Fact]
        public async Task Login_WhenClientReturnsToken_ReturnsOkWithToken()
        {
            // Arrange
            var request = _fixture.Create<TokenRequestDto>();
            var jwt = _fixture.Create<TokenResponseDto>();

            _service.GetTokenAsync(request, default).Returns(jwt);

            // Act
            var result = await _sut.Login(request, default);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(jwt);

            await _service.Received(1).GetTokenAsync(request, default);
        }
    }
}