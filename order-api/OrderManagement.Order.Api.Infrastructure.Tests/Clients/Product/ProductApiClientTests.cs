using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using OrderManagement.Order.Api.Domain.ValueObjects;
using OrderManagement.Order.Api.Infrastructure.Clients.Product;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OrderManagement.Order.Api.Infrastructure.Tests.Clients.Product
{
    public class ProductApiClientTests
    {
        private readonly Fixture _fixture = new();

        private readonly IMapper _mapper = Substitute.For<IMapper>();
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

            _fixture.Inject(_mapper);
            _fixture.Inject(_factory);
            _sut = _fixture.Create<ProductApiClient>();
        }

        [Fact]
        public async Task GetRangeAsync_ShouldSendPostToExpectedUrlWithJsonBody()
        {
            // Arrange
            var dto = _fixture.Create<ProductRange>();
            var expectedJson = JsonSerializer.Serialize(dto);

            _handler.Responder = req =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                };
            };

            // Act
            await _sut.GetProductsAsync(dto, default);

            // Assert
            _handler.LastRequest.Should().NotBeNull();
            _handler.LastRequest!.RequestUri!.ToString().Should().EndWith("api/products/range");

            _handler.LastRequest.Method.Should().Be(HttpMethod.Post);

            var sentJson = await _handler.LastRequest.Content!.ReadAsStringAsync();
            sentJson.Should().BeEquivalentTo(expectedJson);
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