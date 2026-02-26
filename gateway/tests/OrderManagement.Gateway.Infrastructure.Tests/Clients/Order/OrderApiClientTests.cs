using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Orders;
using OrderManagement.Gateway.Infrastructure.Clients.Order;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Gateway.Infrastructure.Tests.Clients.Order
{
    public class OrderApiClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IHttpClientFactory _factory = Substitute.For<IHttpClientFactory>();
        private readonly RecordingHandler _handler = new();
        private readonly HttpClient _httpClient;

        private readonly OrderApiClient _sut;

        public OrderApiClientTests()
        {
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://order.test/")
            };

            _factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

            _fixture.Inject(_factory);
            _sut = _fixture.Create<OrderApiClient>();
        }

        [Fact]
        public async Task ExistsAsync_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Not Found",
                    Content = new StringContent("nope", Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_WhenSuccess_ReturnsBooleanFromBody()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var expected = _fixture.Create<bool>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(expected ? "true" : "false", Encoding.UTF8, "application/json")
                };
            };

            // Act
            var result = await _sut.ExistsAsync(id, default);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task ExistsAsync_WhenNonSuccessAndNotFound_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    ReasonPhrase = "Bad Gateway",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.ExistsAsync(id, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Order API failed:");
            ex.Message.Should().Contain("502");
            ex.Message.Should().Contain("Bad Gateway");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task ListAsync_WhenSuccess_ReturnsList()
        {
            // Arrange
            var dto = _fixture.CreateMany<OrderDto>(3).ToArray();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith("api/orders/list");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            };

            // Act
            var result = await _sut.ListAsync(default);

            // Assert
            result.Should().HaveCount(dto.Length);
        }

        [Fact]
        public async Task ListAsync_WhenBodyIsNullJson_ThrowsInvalidOperationException()
        {
            // Arrange
            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith("api/orders/list");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                };
            };

            // Act
            var act = () => _sut.ListAsync(default);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            ex.Message.Should().Be("Failed to deserialize response.");
        }

        [Fact]
        public async Task ListAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith("api/orders/list");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Internal Server Error",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.ListAsync(default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Order API failed:");
            ex.Message.Should().Contain("500");
            ex.Message.Should().Contain("Internal Server Error");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task GetAsync_WhenSuccess_ReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var dto = _fixture.Create<OrderDto>();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            };

            // Act
            var result = await _sut.GetAsync(id, default);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_WhenBodyIsNullJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                };
            };

            // Act
            var act = () => _sut.GetAsync(id, default);

            // Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
            ex.Message.Should().Be("Failed to deserialize response.");
        }

        [Fact]
        public async Task GetAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Not Found",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.GetAsync(id, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Order API failed:");
            ex.Message.Should().Contain("404");
            ex.Message.Should().Contain("Not Found");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateAsync(null!, default));
            ex.ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task CreateAsync_WhenSuccess_SendsPostToExpectedUrlWithJsonBody()
        {
            // Arrange
            var dto = _fixture.Create<CreateOrderDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/orders");

                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            // Act
            await _sut.CreateAsync(dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith("api/orders");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
        }

        [Fact]
        public async Task CreateAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var dto = _fixture.Create<CreateOrderDto>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/orders");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Bad Request",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.CreateAsync(dto, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("400");
            ex.Message.Should().Contain("Bad Request");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(id, null!, default));
            ex.ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task UpdateAsync_WhenSuccess_SendsPutToExpectedUrlWithJsonBody()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var dto = _fixture.Create<UpdateOrderDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            };

            // Act
            await _sut.UpdateAsync(id, dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
        }

        [Fact]
        public async Task UpdateAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var dto = _fixture.Create<UpdateOrderDto>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    ReasonPhrase = "Conflict",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.UpdateAsync(id, dto, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("409");
            ex.Message.Should().Contain("Conflict");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task DeleteAsync_WhenSuccess_SendsDeleteToExpectedUrl()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Delete);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            };

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
        }

        [Fact]
        public async Task DeleteAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Delete);
                req.RequestUri!.ToString().Should().EndWith($"api/orders/{id}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Internal Server Error",
                    Content = new StringContent(body, Encoding.UTF8, "text/plain")
                };
            };

            // Act
            var act = () => _sut.DeleteAsync(id, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("500");
            ex.Message.Should().Contain("Internal Server Error");
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