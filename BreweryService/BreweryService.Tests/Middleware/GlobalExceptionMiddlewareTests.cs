using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BreweryService.Middleware;

namespace BreweryService.Tests.Middleware
{
    public class GlobalExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_NoException_CallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            bool nextCalled = false;

            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new GlobalExceptionMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal(200, context.Response.StatusCode); // default if no error
        }

        [Fact]
        public async Task InvokeAsync_ExceptionThrown_ReturnsJsonErrorResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream(); // capture response body

            RequestDelegate next = (ctx) =>
            {
                throw new InvalidOperationException("Test exception");
            };

            var middleware = new GlobalExceptionMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

            Assert.Equal("application/json; charset=utf-8", context.Response.ContentType);
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

            var json = JsonSerializer.Deserialize<JsonElement>(body);

            Assert.Equal(500, json.GetProperty("statusCode").GetInt32());
            Assert.Equal("An unexpected error occurred.", json.GetProperty("message").GetString());
            Assert.Equal("Test exception", json.GetProperty("details").GetString());
        }

    }
}
