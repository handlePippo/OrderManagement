using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Products;
using OrderManagement.Gateway.Infrastructure.Clients.Product;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Gateway.Infrastructure.Tests.Clients.Product
{
    public class ProductApiClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IHttpClientFactory _factory = Substitute.For<IHttpClientFactory>();
        private readonly RecordingHandler _handler = new();
        private readonly HttpClient _httpClient;

        private readonly ProductApiClient _sut;

        public ProductApiClientTests()
        {
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://product.test/")
            };

            _factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

            _fixture.Inject(_factory);
            _sut = _fixture.Create<ProductApiClient>();
        }

        [Fact]
        public async Task ExistsAsync_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
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
            var id = _fixture.Create<int>();
            var expected = _fixture.Create<bool>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
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
            var id = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
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
            ex.Message.Should().Contain("Product API failed:");
            ex.Message.Should().Contain("502");
            ex.Message.Should().Contain("Bad Gateway");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task ListAsync_WhenSuccess_ReturnsList()
        {
            // Arrange
            var dto = _fixture.CreateMany<ProductDto>(3).ToArray();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith("api/products/list");
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
                req.RequestUri!.ToString().Should().EndWith("api/products/list");
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
                req.RequestUri!.ToString().Should().EndWith("api/products/list");
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
            ex.Message.Should().Contain("Product API failed:");
            ex.Message.Should().Contain("500");
            ex.Message.Should().Contain("Internal Server Error");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task GetAsync_WhenSuccess_ReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<ProductDto>();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
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
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
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
            var id = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
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
            ex.Message.Should().Contain("Product API failed:");
            ex.Message.Should().Contain("404");
            ex.Message.Should().Contain("Not Found");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task GetRangeAsync_ShouldSendPostToExpectedUrlWithJsonBody()
        {
            // Arrange
            var dto = _fixture.Create<GetProductRangeDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                };
            };

            // Act
            await _sut.GetRangeAsync(dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.RequestUri!.ToString().Should().EndWith("api/products/range");

            _handler.LastRequest.Method.Should().Be(HttpMethod.Post);

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
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
            var dto = _fixture.Create<CreateProductDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req => new HttpResponseMessage(HttpStatusCode.OK);

            // Act
            await _sut.CreateAsync(dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith("api/products");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
        }

        [Fact]
        public async Task UpdateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(id, null!, default));
            ex.ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task UpdateAsync_WhenSuccess_SendsPutToExpectedUrlWithJsonBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateProductDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req => new HttpResponseMessage(HttpStatusCode.NoContent);

            // Act
            await _sut.UpdateAsync(id, dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/products/{id}");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
        }

        [Fact]
        public async Task DeleteAsync_WhenSuccess_SendsDeleteToExpectedUrl()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req => new HttpResponseMessage(HttpStatusCode.NoContent);

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/products/{id}");
        }

        [Fact]
        public async Task IncreaseStock_WhenSuccess_SendsPutToExpectedUrl()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var qty = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}/stock/increase/{qty}");
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            };

            // Act
            await _sut.IncreaseStock(id, qty, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/products/{id}/stock/increase/{qty}");
        }

        [Fact]
        public async Task IncreaseStock_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var qty = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req => new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "Bad Request",
                Content = new StringContent(body, Encoding.UTF8, "text/plain")
            };

            // Act
            var act = () => _sut.IncreaseStock(id, qty, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Product API failed:");
            ex.Message.Should().Contain("400");
            ex.Message.Should().Contain("Bad Request");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task DecreaseStock_WhenSuccess_SendsPutToExpectedUrl()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var qty = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/products/{id}/stock/decrease/{qty}");
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            };

            // Act
            await _sut.DecreaseStock(id, qty, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/products/{id}/stock/decrease/{qty}");
        }

        [Fact]
        public async Task DecreaseStock_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var qty = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req => new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                ReasonPhrase = "Not Found",
                Content = new StringContent(body, Encoding.UTF8, "text/plain")
            };

            // Act
            var act = () => _sut.DecreaseStock(id, qty, default);

            // Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(act);
            ex.Message.Should().Contain("Product API failed:");
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