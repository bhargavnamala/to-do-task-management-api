using System.Net;
using System.Text.Json;

namespace ToDoTaskManagement.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var userId = context.User?.FindFirst("sub")?.Value ?? "Anonymous";
            var correlationId = Guid.NewGuid().ToString();
            var path = context.Request.Path;

            _logger.LogError(
                ex,
                "Unhandled exception | User: {UserId} | Path: {Path} | CorrelationId: {CorrelationId}",
                userId,
                path,
                correlationId
            );

            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                success = false,
                error = ex.Message,
                correlationId,
                status = statusCode.ToString()
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
