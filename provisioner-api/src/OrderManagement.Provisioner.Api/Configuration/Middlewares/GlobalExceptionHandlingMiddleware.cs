using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Provisioner.Api.Configuration.Middlewares
{
    /// <summary>
    /// Middleware to handle exceptions in the whole application.
    /// </summary>
    public sealed class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context is null || next is null)
            {
                return;
            }

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}