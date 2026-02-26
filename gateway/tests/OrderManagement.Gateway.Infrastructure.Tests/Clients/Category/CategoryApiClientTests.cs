using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Categories;
using OrderManagement.Gateway.Infrastructure.Clients.Category;
using System.Net;
using System.Text;

namespace OrderManagement.Gateway.Infrastructure.Tests.Clients.Category
{
    public class CategoryApiClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IHttpClientFactory _factory = Substitute.For<IHttpClientFactory>();
        private readonly RecordingHandler _handler = new();
        private readonly HttpClient _httpClient;

        private readonly CategoryApiClient _sut;

        public CategoryApiClientTests()
        {
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://category.test/")
            };

            _factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

            _fixture.Inject(_factory);
            _sut = _fixture.Create<CategoryApiClient>();
        }

        [Fact]
        public async Task ExistsAsync_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
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
            ex.Message.Should().Contain("Category API failed:");
            ex.Message.Should().Contain("502");
            ex.Message.Should().Contain("Bad Gateway");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task ListAsync_WhenSuccess_ReturnsList()
        {
            // Arrange
            var dto = _fixture.CreateMany<CategoryDto>(3).ToArray();
            var json = System.Text.Json.JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith("api/categories/list");
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
                req.RequestUri!.ToString().Should().EndWith("api/categories/list");
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
                req.RequestUri!.ToString().Should().EndWith("api/categories/list");
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
            ex.Message.Should().Contain("Category API failed:");
            ex.Message.Should().Contain("500");
            ex.Message.Should().Contain("Internal Server Error");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task GetAsync_WhenSuccess_ReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<CategoryDto>();
            var json = System.Text.Json.JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
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
            ex.Message.Should().Contain("Category API failed:");
            ex.Message.Should().Contain("404");
            ex.Message.Should().Contain("Not Found");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task CreateAsync_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateAsync(null!, default));
            (await ex).ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task CreateAsync_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            var dto = _fixture.Create<CreateCategoryDto>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/categories");
                return new HttpResponseMessage(HttpStatusCode.OK);
            };

            // Act
            var act = () => _sut.CreateAsync(dto, default);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CreateAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var dto = _fixture.Create<CreateCategoryDto>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Post);
                req.RequestUri!.ToString().Should().EndWith("api/categories");
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
            var id = _fixture.Create<int>();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(id, null!, default));
            (await ex).ParamName.Should().Be("dto");
        }

        [Fact]
        public async Task UpdateAsync_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateCategoryDto>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            };

            // Act
            var act = () => _sut.UpdateAsync(id, dto, default);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_WhenSuccess_DoesNotThrow()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Delete);
                req.RequestUri!.ToString().Should().EndWith($"api/categories/{id}");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            };

            // Act
            var act = () => _sut.DeleteAsync(id, default);

            // Assert
            await act.Should().NotThrowAsync();
        }

        private sealed class RecordingHandler : HttpMessageHandler
        {
            public Func<HttpRequestMessage, HttpResponseMessage>? Responder { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (Responder is null)
                {
                    throw new InvalidOperationException("Responder not set.");
                }

                return Task.FromResult(Responder(request));
            }
        }
    }
}