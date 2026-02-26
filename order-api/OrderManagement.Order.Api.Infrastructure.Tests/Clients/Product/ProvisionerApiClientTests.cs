using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Infrastructure.Clients.Provisioner;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Order.Api.Infrastructure.Tests.Clients.Product
{
    public class ProvisionerApiClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IHttpClientFactory _factory = Substitute.For<IHttpClientFactory>();
        private readonly RecordingHandler _handler = new();
        private readonly HttpClient _httpClient;

        private readonly ProvisionerApiClient _sut;

        public ProvisionerApiClientTests()
        {
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://product.test/")
            };

            _factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

            _fixture.Inject(_mapper);
            _fixture.Inject(_factory);
            _sut = _fixture.Create<ProvisionerApiClient>();
        }

        [Fact]
        public async Task Address_GetAsync_WhenSuccess_ReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();

            var dto = _fixture.Create<ApiShippingAddress>();
            var response = _fixture.Create<ShippingAddress>();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/addresses/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            };

            _mapper.Map<ShippingAddress>(Arg.Is<ApiShippingAddress>(a => a.Id == dto.Id)).Returns(response);

            // Act
            var result = await _sut.GetShippingAddressAsync(id, default);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Address_GetAsync_WhenBodyIsNullJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/addresses/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                };
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetShippingAddressAsync(id, default));
            ex.Message.Should().Be("Failed to deserialize response.");
        }

        [Fact]
        public async Task Address_GetAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/addresses/{id}");
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Not Found",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => _sut.GetShippingAddressAsync(id, default));
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("404");
            ex.Message.Should().Contain("Not Found");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task User_GetAsync_WhenSuccess_ReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<ApiUser>();
            var response = _fixture.Create<User>();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            };

            _mapper.Map<User>(Arg.Is<ApiUser>(a => a.Id == dto.Id)).Returns(response);

            // Act
            var result = await _sut.GetUserAsync(id, default);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task User_GetAsync_WhenBodyIsNullJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                };
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetUserAsync(id, default));
            ex.Message.Should().Be("Failed to deserialize response.");
        }

        [Fact]
        public async Task User_GetAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Not Found",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => _sut.GetUserAsync(id, default));
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("404");
            ex.Message.Should().Contain("Not Found");
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
