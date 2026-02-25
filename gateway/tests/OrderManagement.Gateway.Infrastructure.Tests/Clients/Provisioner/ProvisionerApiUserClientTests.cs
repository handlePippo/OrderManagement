using System.Net;
using System.Text;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Persistence.Clients.Provisioner;
using Xunit;

namespace OrderManagement.Gateway.Infrastructure.Tests.Clients.Provisioner
{
    public class ProvisionerApiUserClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IHttpClientFactory _factory = Substitute.For<IHttpClientFactory>();
        private readonly RecordingHandler _handler = new();
        private readonly HttpClient _httpClient;

        private readonly ProvisionerApiUserClient _sut;

        public ProvisionerApiUserClientTests()
        {
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://provisioner.test/")
            };

            _factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

            _fixture.Inject(_factory);
            _sut = _fixture.Create<ProvisionerApiUserClient>();
        }

        [Fact]
        public async Task ExistsAsync_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("502");
            ex.Message.Should().Contain("Bad Gateway");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task ListAsync_WhenSuccess_ReturnsList()
        {
            // Arrange
            var dto = _fixture.CreateMany<UserDto>(3).ToArray();
            var json = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Get);
                req.RequestUri!.ToString().Should().EndWith("api/users/list");
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
                req.RequestUri!.ToString().Should().EndWith("api/users/list");
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
                req.RequestUri!.ToString().Should().EndWith("api/users/list");
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
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("500");
            ex.Message.Should().Contain("Internal Server Error");
            ex.Message.Should().Contain(body);
        }

        [Fact]
        public async Task GetAsync_WhenSuccess_ReturnsDto()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UserDto>();
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
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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
            ex.Message.Should().Contain("Provisioner API failed:");
            ex.Message.Should().Contain("404");
            ex.Message.Should().Contain("Not Found");
            ex.Message.Should().Contain(body);
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
            var dto = _fixture.Create<UpdateUserDto>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            };

            // Act
            await _sut.UpdateAsync(id, dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Put);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/users/{id}");

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
        }

        [Fact]
        public async Task UpdateAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateUserDto>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Put);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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
            var id = _fixture.Create<int>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Delete);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            };

            // Act
            await _sut.DeleteAsync(id, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.Method.Should().Be(HttpMethod.Delete);
            _handler.LastRequest.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
        }

        [Fact]
        public async Task DeleteAsync_WhenNonSuccess_ThrowsHttpRequestExceptionWithBody()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var body = _fixture.Create<string>();

            _handler.Responder = req =>
            {
                req.Method.Should().Be(HttpMethod.Delete);
                req.RequestUri!.ToString().Should().EndWith($"api/users/{id}");
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