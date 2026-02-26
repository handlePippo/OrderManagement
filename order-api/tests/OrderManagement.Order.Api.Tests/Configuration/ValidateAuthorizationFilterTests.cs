
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Configuration;

namespace OrderManagement.Order.Api.Tests.Configuration
{
    public sealed class ValidateAuthorizationFilterTests : IDisposable
    {
        private readonly Fixture _fixture = new();

        private ValidateAuthorizationFilter _sut;

        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
        private readonly IMemoryCache _cache;

        public ValidateAuthorizationFilterTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());

            _fixture.Inject(_currentUserProvider);
            _fixture.Inject(_orderRepository);
            _fixture.Inject(_cache);

            _sut = _fixture.Create<ValidateAuthorizationFilter>();
        }

        public void Dispose()
        {
            (_cache as MemoryCache)?.Dispose();
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenIdArgumentMissing_ReturnsBadRequest_AndDoesNotCallNext()
        {
            // Arrange
            var ctx = CreateContext(actionArgs: new Dictionary<string, object?>());
            var nextCalled = false;

            Task<ActionExecutedContext> Next()
            {
                nextCalled = true;
                return Task.FromResult(CreateExecutedContext(ctx));
            }

            // Act
            await _sut.OnActionExecutionAsync(ctx, Next);

            // Assert
            ctx.Result.Should().BeOfType<BadRequestResult>();
            nextCalled.Should().BeFalse();

            await _orderRepository.DidNotReceiveWithAnyArgs().GetAsync(default, default);
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenIdArgumentIsNotGuid_ReturnsBadRequest()
        {
            // Arrange
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = "not-a-guid" });
            var nextCalled = false;

            Task<ActionExecutedContext> Next()
            {
                nextCalled = true;
                return Task.FromResult(CreateExecutedContext(ctx));
            }

            // Act
            await _sut.OnActionExecutionAsync(ctx, Next);

            // Assert
            ctx.Result.Should().BeOfType<BadRequestResult>();
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenOrderNotFound_ReturnsForbid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            _orderRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Domain.Entities.Order?)null);

            _currentUserProvider.GetLoggedUserId().Returns(_fixture.Create<int>());
            _currentUserProvider.IsAdmin.Returns(false);

            var nextCalled = false;
            Task<ActionExecutedContext> Next()
            {
                nextCalled = true;
                return Task.FromResult(CreateExecutedContext(ctx));
            }

            // Act
            await _sut.OnActionExecutionAsync(ctx, Next);

            // Assert
            ctx.Result.Should().BeOfType<ForbidResult>();
            nextCalled.Should().BeFalse();

            await _orderRepository.Received(1).GetAsync(id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenUserIsNotOwnerAndNotAdmin_ReturnsForbid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var ownerUserId = 100;
            var currentUserId = 200;

            var order = new Domain.Entities.Order();
            order.SetUserId(ownerUserId);

            _orderRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(order);

            _currentUserProvider.GetLoggedUserId().Returns(currentUserId);
            _currentUserProvider.IsAdmin.Returns(false);

            var nextCalled = false;
            Task<ActionExecutedContext> Next()
            {
                nextCalled = true;
                return Task.FromResult(CreateExecutedContext(ctx));
            }

            // Act
            await _sut.OnActionExecutionAsync(ctx, Next);

            // Assert
            ctx.Result.Should().BeOfType<ForbidResult>();
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenUserIsOwner_AllowsExecution()
        {
            // Arrange
            var id = Guid.NewGuid();
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var userId = _fixture.Create<int>();

            var order = new Domain.Entities.Order();
            order.SetUserId(userId);

            _orderRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(order);

            _currentUserProvider.GetLoggedUserId().Returns(userId);
            _currentUserProvider.IsAdmin.Returns(false);

            var nextCalled = false;
            Task<ActionExecutedContext> Next()
            {
                nextCalled = true;
                return Task.FromResult(CreateExecutedContext(ctx));
            }

            // Act
            await _sut.OnActionExecutionAsync(ctx, Next);

            // Assert
            ctx.Result.Should().BeNull();
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenAdmin_AllowsExecution_EvenIfNotOwner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var ownerUserId = 10;
            var currentUserId = 99;

            var order = new Domain.Entities.Order();
            order.SetUserId(ownerUserId);

            _orderRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(order);

            _currentUserProvider.GetLoggedUserId().Returns(currentUserId);
            _currentUserProvider.IsAdmin.Returns(true);

            var nextCalled = false;
            Task<ActionExecutedContext> Next()
            {
                nextCalled = true;
                return Task.FromResult(CreateExecutedContext(ctx));
            }

            // Act
            await _sut.OnActionExecutionAsync(ctx, Next);

            // Assert
            ctx.Result.Should().BeNull();
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task OnActionExecutionAsync_UsesCache_SecondCallDoesNotHitRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var ctx1 = CreateContext(new Dictionary<string, object?> { ["id"] = id });
            var ctx2 = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var userId = _fixture.Create<int>();

            var order = new Domain.Entities.Order();
            order.SetUserId(userId);

            _orderRepository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(order);

            _currentUserProvider.GetLoggedUserId().Returns(userId);
            _currentUserProvider.IsAdmin.Returns(false);

            // Act
            await _sut.OnActionExecutionAsync(ctx1, () => Task.FromResult(CreateExecutedContext(ctx1)));
            await _sut.OnActionExecutionAsync(ctx2, () => Task.FromResult(CreateExecutedContext(ctx2)));

            // Assert
            ctx1.Result.Should().BeNull();
            ctx2.Result.Should().BeNull();

            await _orderRepository.Received(1).GetAsync(id, Arg.Any<CancellationToken>());
        }

        private static ActionExecutingContext CreateContext(Dictionary<string, object?> actionArgs)
        {
            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            return new ActionExecutingContext(
                actionContext,
                filters: new List<IFilterMetadata>(),
                actionArguments: actionArgs,
                controller: new object());
        }

        private static ActionExecutedContext CreateExecutedContext(ActionExecutingContext executingContext)
        {
            return new ActionExecutedContext(
                executingContext,
                filters: new List<IFilterMetadata>(),
                controller: new object());
        }
    }
}
