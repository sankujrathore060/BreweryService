using BreweryService.Data.Repository;
using BreweryService.Data.Helper;
using BreweryService.Data.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace BreweryService.Service.Tests.Service
{
    public class BreweryServiceTests
    {
        private readonly Mock<IBreweryRepository> _repositoryMock;
        private readonly Mock<ILogger<IBreweryService>> _loggerMock;
        private readonly BreweryService _service;

        public BreweryServiceTests()
        {
            _repositoryMock = new Mock<IBreweryRepository>();
            _loggerMock = new Mock<ILogger<IBreweryService>>();
            _service = new BreweryService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetBreweriesAsync_ReturnsBreweries()
        {
            // Arrange
            var filter = new BreweryFilter { PageSize = 1 };
            var breweries = new List<Brewery>
            {
                new Brewery { Id = "1", Name = "Test Brewery", City = "New York" }
            };

            _repositoryMock
                .Setup(r => r.GetAllBreweriesAsync(filter))
                .ReturnsAsync(breweries);

            // Act
            var result = await _service.GetBreweriesAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Brewery", ((List<Brewery>)result)[0].Name);

            _repositoryMock.Verify(r => r.GetAllBreweriesAsync(filter), Times.Once);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAutocompleteSuggestionsAsync_ReturnsTop10DistinctSuggestions()
        {
            // Arrange
            var filter = new BreweryFilter { PageSize = 20 };
            var breweries = new List<Brewery>();

            // Add 15 breweries with duplicates
            for (int i = 1; i <= 15; i++)
            {
                breweries.Add(new Brewery { Id = i.ToString(), Name = $"Brewery {i}" });
            }
            breweries.Add(new Brewery { Id = "1", Name = "Brewery 1" }); // duplicate

            _repositoryMock
                .Setup(r => r.GetAllBreweriesAsync(filter))
                .ReturnsAsync(breweries);

            // Act
            var result = await _service.GetAutocompleteSuggestionsAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count <= 10); // ensures Take(10)
            Assert.Contains("1", result.Keys);
            Assert.Contains("Brewery 1", result.Values);

            _repositoryMock.Verify(r => r.GetAllBreweriesAsync(filter), Times.Once);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAutocompleteSuggestionsAsync_ReturnsEmpty_WhenNoBreweries()
        {
            // Arrange
            var filter = new BreweryFilter { PageSize = 5 };
            _repositoryMock
                .Setup(r => r.GetAllBreweriesAsync(filter))
                .ReturnsAsync(new List<Brewery>());

            // Act
            var result = await _service.GetAutocompleteSuggestionsAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _repositoryMock.Verify(r => r.GetAllBreweriesAsync(filter), Times.Once);
        }
    }
}
