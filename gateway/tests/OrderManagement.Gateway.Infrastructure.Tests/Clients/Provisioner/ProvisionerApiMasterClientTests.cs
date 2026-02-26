using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Token;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Infrastructure.Clients.Provisioner;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Gateway.Infrastructure.Tests.Clients.Provisioner
{
    public class ProvisionerApiMasterClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IHttpClientFactory _factory = Substitute.For<IHttpClientFactory>();
        private readonly RecordingHandler _handler = new();
        private readonly HttpClient _httpClient;

        private readonly ProvisionerApiMasterClient _sut;

        public ProvisionerApiMasterClientTests()
        {
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://provisioner-master.test/")
            };

            _factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

            _fixture.Inject(_factory);
            _sut = _fixture.Create<ProvisionerApiMasterClient>();
        }

        [Fact]
        public async Task CreateUserAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateUserAsync(null!, default));
            ex.ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task CreateUserAsync_WhenSuccess_SendsPostToExpectedUrlWithJsonBody()
        {
            // Arrange
            var dto = _fixture.Create<CreateUserDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/users");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            // Act
            await _sut.CreateUserAsync(dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith("api/users");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
        }

        [Fact]
        public async Task CreateUserAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var dto = _fixture.Create<CreateUserDto>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/users");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Bad Request",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.CreateUserAsync(dto, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("400");
            ex.Message.Should().Contain("Bad Request");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task GetTokenAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetTokenAsync(null!, default));
            ex.ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task GetTokenAsync_WhenSuccess_SendsPostToExpectedUrlWithJsonBody_AndReturnsToken()
        {
            // Arrange
            var dto = _fixture.Create<TokenRequestDto>();
            var expectedRequestJson = JsonSerializer.Serialize(dto);

            var tokenResponse = _fixture.Create<TokenResponseDto>();
            var responseJson = JsonSerializer.Serialize(tokenResponse);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/token/login");

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                };
            };

            // Act
            var result = await _sut.GetTokenAsync(dto, default);

            // Assert
            result.Should().NotBeNull();

            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith("api/token/login");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedRequestJson);
        }

        [Fact]
        public async Task GetTokenAsync_WhenBodyIsNullJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = _fixture.Create<TokenRequestDto>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/token/login");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                };
            };

            // Act
            var act = () => _sut.GetTokenAsync(dto, default);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            ex.Message.Should().Be("Failed to deserialize response.");
        }

        [Fact]
        public async Task GetTokenAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var dto = _fixture.Create<TokenRequestDto>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/token/login");
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Unauthorized",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.GetTokenAsync(dto, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("401");
            ex.Message.Should().Contain("Unauthorized");
            ex.Message.Should().Contain(body);
        }

        private sealed class RecordingHandler : HttpMessageHandler
        {
            public Func<HttpRequestMessage, HttpResponseMessage>? Responder { get; set; }
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;

                if (Responder is null)
                {
                    throw new InvalidOperationException("Responder not set.");
                }

                return Task.FromResult(Responder(request));
            }
        }
    }
}