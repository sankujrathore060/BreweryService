using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BreweryService.Data.Repository;
using BreweryService.Data.Entity;
using BreweryService.Data.Helper;
using BreweryService.Common;
using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Caching.Memory;

namespace BreweryService.Data.Tests.Repository
{
    public class BreweryRepositoryTests
    {
        private BreweryServiceAppSettings CreateSettings() =>
            new BreweryServiceAppSettings
            {
                OpenbrewerydbURL = "https://api.openbrewerydb.org/breweries",
                CacheKey = "BreweryCacheKey"
            };

        private HttpClient CreateHttpClient(HttpResponseMessage responseMessage)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task GetAllBreweriesAsync_ReturnsData_AndCachesIt()
        {
            // Arrange
            var breweries = new List<Brewery>
            {
                new Brewery { Id = "1", Name = "Test Brewery", City = "New York" }
            };

            var json = JsonSerializer.Serialize(breweries);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var httpClient = CreateHttpClient(responseMessage);

            var options = Options.Create(CreateSettings());
            var logger = Mock.Of<ILogger<IBreweryRepository>>();

            var repository = new BreweryRepository(memoryCache, httpClient, options, logger);

            var filter = new BreweryFilter { PageSize = 1 };

            // Act
            var result = await repository.GetAllBreweriesAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Brewery", ((List<Brewery>)result)[0].Name);

            // Verify caching by calling again
            var result2 = await repository.GetAllBreweriesAsync(filter);
            Assert.Single(result2); // from cache
        }

        [Fact]
        public async Task GetAllBreweriesAsync_ThrowsException_OnFailure()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var httpClient = CreateHttpClient(responseMessage);

            var options = Options.Create(CreateSettings());
            var logger = Mock.Of<ILogger<IBreweryRepository>>();

            var repository = new BreweryRepository(memoryCache, httpClient, options, logger);

            var filter = new BreweryFilter { PageSize = 1 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => repository.GetAllBreweriesAsync(filter));
            Assert.Equal("Internal Server Error", ex.Message);
        }

        [Fact]
        public async Task GetAllBreweriesAsync_ReturnsEmptyList_WhenResponseNotArray()
        {
            // Arrange
            var json = "{}"; // not a list
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var httpClient = CreateHttpClient(responseMessage);

            var options = Options.Create(CreateSettings());
            var logger = Mock.Of<ILogger<IBreweryRepository>>();

            var repository = new BreweryRepository(memoryCache, httpClient, options, logger);

            var filter = new BreweryFilter { PageSize = 1 };

            // Act
            var result = await repository.GetAllBreweriesAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
