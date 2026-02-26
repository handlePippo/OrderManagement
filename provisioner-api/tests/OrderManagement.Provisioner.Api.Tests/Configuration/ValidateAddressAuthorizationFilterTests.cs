using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Configuration;
using OrderManagement.Provisioner.Api.Domain.Entities;

namespace OrderManagement.Provisioner.Api.Tests.Configuration
{
    public sealed class ValidateAddressAuthorizationFilterTests : IDisposable
    {
        private readonly Fixture _fixture = new();
        private readonly ValidateAddressAuthorizationFilter _sut;
        private readonly ICurrentUserProvider _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        private readonly IAddressRepository _repository = Substitute.For<IAddressRepository>();
        private readonly IMemoryCache _cache;

        public ValidateAddressAuthorizationFilterTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());

            _fixture.Inject(_currentUserProvider);
            _fixture.Inject(_repository);
            _fixture.Inject(_cache);

            _sut = _fixture.Create<ValidateAddressAuthorizationFilter>();
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
            ctx.Result.Should().BeOfType<ForbidResult>();
            nextCalled.Should().BeFalse();

            await _repository.DidNotReceiveWithAnyArgs().GetAsync(id: default, default);
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
            ctx.Result.Should().BeOfType<ForbidResult>();
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenOrderNotFound_ReturnsForbid()
        {
            // Arrange
            var id = 123;
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Address?)null);

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

            await _repository.Received(1).GetAsync(id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenUserIsNotOwnerAndNotAdmin_ReturnsForbid()
        {
            // Arrange
            var id = 123;
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var ownerUserId = 100;
            var currentUserId = 200;

            var address = new Address(id, ownerUserId);

            _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(address);

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
            var id = 123;
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var userId = _fixture.Create<int>();

            var address = new Address(id, userId);

            _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(address);

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
            var id = 123;
            var ctx = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var ownerUserId = 10;
            var currentUserId = 99;

            var address = new Address(id, ownerUserId);

            _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(address);

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
            var id = 123;
            var ctx1 = CreateContext(new Dictionary<string, object?> { ["id"] = id });
            var ctx2 = CreateContext(new Dictionary<string, object?> { ["id"] = id });

            var userId = _fixture.Create<int>();
            var address = new Address(id, userId);

            _repository.GetAsync(id, default).Returns(address);

            _currentUserProvider.GetLoggedUserId().Returns(userId);
            _currentUserProvider.IsAdmin.Returns(false);

            // Act
            await _sut.OnActionExecutionAsync(ctx1, () => Task.FromResult(CreateExecutedContext(ctx1)));
            await _sut.OnActionExecutionAsync(ctx2, () => Task.FromResult(CreateExecutedContext(ctx2)));

            // Assert
            ctx1.Result.Should().BeNull();
            ctx2.Result.Should().BeNull();

            await _repository.Received(1).GetAsync(id, default);
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
