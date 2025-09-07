using Xunit;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using BreweryService.Middleware;

namespace BreweryService.Tests.Middleware
{
    public class BreweryServiceCorsMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_AddsCorsHeaders_ForGetRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";

            bool nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new BreweryServiceCorsMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal("*", context.Response.Headers["Access-Control-Allow-Origin"]);
            Assert.Contains("GET", (string)context.Response.Headers.AccessControlAllowMethods);
            Assert.Equal("*", context.Response.Headers["Access-Control-Allow-Headers"]);
        }

        [Fact]
        public async Task InvokeAsync_Returns200_ForOptionsRequest_AndDoesNotCallNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "OPTIONS";

            bool nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new BreweryServiceCorsMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(nextCalled); // next should NOT be called
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
            Assert.Equal("*", context.Response.Headers["Access-Control-Allow-Origin"]);
            Assert.Contains("OPTIONS", (string)context.Response.Headers.AccessControlAllowMethods);
            Assert.Equal("*", context.Response.Headers["Access-Control-Allow-Headers"]);
        }
    }
}
