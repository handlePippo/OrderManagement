using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OrderManagement.Gateway.Configuration.Middlewares;
using System.Text.Json;

namespace OrderManagement.Gateway.Tests.Configuration.Middlewares
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        private readonly Fixture _fixture = new();
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger =
            Substitute.For<ILogger<GlobalExceptionHandlingMiddleware>>();

        private readonly GlobalExceptionHandlingMiddleware _sut;

        public GlobalExceptionHandlingMiddlewareTests()
        {
            _fixture.Inject(_logger);
            _sut = _fixture.Create<GlobalExceptionHandlingMiddleware>();
        }

        [Fact]
        public async Task InvokeAsync_WhenNextDoesNotThrow_DoesNotSet500AndDoesNotWriteProblem()
        {
            // Arrange
            var context = CreateHttpContextWithBody();
            RequestDelegate next = _ => Task.CompletedTask;

            // Act
            await _sut.InvokeAsync(context, next);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            context.Response.Body.Length.Should().Be(0);

            _logger.DidNotReceiveWithAnyArgs().Log(default, default, default!, default!, default!);
        }

        [Fact]
        public async Task InvokeAsync_WhenNextThrows_WritesProblemDetailsAndSets500()
        {
            // Arrange
            var context = CreateHttpContextWithBody();
            var ex = new InvalidOperationException(_fixture.Create<string>());

            RequestDelegate next = _ => throw ex;

            // Act
            await _sut.InvokeAsync(context, next);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            context.Response.ContentType.Should().Contain("application/json");

            var problem = await ReadProblemDetailsAsync(context);

            problem.Should().NotBeNull();
            problem!.Title.Should().Be("An unexpected error occurred");
            problem.Status.Should().Be(StatusCodes.Status500InternalServerError);
            problem.Detail.Should().Be(ex.Message);
            problem.Type.Should().Be(ex.GetType().Name);

            _logger.Received(1).Log(LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Is<Exception>(e => ReferenceEquals(e, ex)),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task InvokeAsync_WhenContextIsNull_ReturnsWithoutThrowing()
        {
            // Arrange
            RequestDelegate next = _ => Task.CompletedTask;

            // Act
            var act = () => _sut.InvokeAsync(null!, next);

            // Assert
            await act.Should().NotThrowAsync();
            _logger.DidNotReceiveWithAnyArgs().Log(default, default, default!, default!, default!);
        }

        [Fact]
        public async Task InvokeAsync_WhenNextIsNull_ReturnsWithoutThrowing()
        {
            // Arrange
            var context = CreateHttpContextWithBody();

            // Act
            var act = () => _sut.InvokeAsync(context, null!);

            // Assert
            await act.Should().NotThrowAsync();
            _logger.DidNotReceiveWithAnyArgs().Log(default, default, default!, default!, default!);
        }

        private static DefaultHttpContext CreateHttpContextWithBody()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task<ProblemDetails?> ReadProblemDetailsAsync(HttpContext context)
        {
            context.Response.Body.Position = 0;

            using var reader = new StreamReader(context.Response.Body, leaveOpen: true);
            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ProblemDetails>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}