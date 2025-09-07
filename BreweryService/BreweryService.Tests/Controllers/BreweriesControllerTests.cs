using Moq;
using BreweryService.Service;
using BreweryService.Controllers;
using Microsoft.Extensions.Logging;
using BreweryService.DTO;
using BreweryService.Data.Entity;
using BreweryService.Data.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BreweryService.Tests.Controllers
{
    public class BreweriesControllerTests
    {
        private readonly Mock<IBreweryService> _breweryServiceMock;
        private readonly Mock<ILogger<BreweriesController>> _loggerMock;
        private readonly BreweriesController _controller;

        public BreweriesControllerTests()
        {
            _breweryServiceMock = new Mock<IBreweryService>();
            _loggerMock = new Mock<ILogger<BreweriesController>>();
            _controller = new BreweriesController(_breweryServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithMappedBreweries()
        {
            // Arrange
            var filter = new BreweryFilterDTO { SearchKey = "name", SearchValue = "Test" };
            var breweries = new List<Brewery>
            {
                new Brewery {Id= "Id1", Name = "Test Brewery", City = "New York", Phone = "123456" }
            };

            _breweryServiceMock
                .Setup(s => s.GetBreweriesAsync(It.IsAny<BreweryFilter>()))
                .ReturnsAsync(breweries);

            // Act
            var result = await _controller.Get(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<BreweryDTO>>(okResult.Value);
            Assert.Single(returned);
            Assert.Equal("Test Brewery", returned.First().Name);
        }

        [Fact]
        public async Task Get_Returns500_OnException()
        {
            // Arrange
            var filter = new BreweryFilterDTO();
            _breweryServiceMock
                .Setup(s => s.GetBreweriesAsync(It.IsAny<BreweryFilter>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.Get(filter);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Internal server error", statusResult.Value);
        }

        [Fact]
        public async Task GetAutocomplete_ReturnsOk_WithSuggestions()
        {
            // Arrange
            var filter = new BreweryFilterDTO { SearchValue = "Ale" };
            var suggestions = new Dictionary<string, string>
            {
                { "1", "Test Brewery 1" },
                { "2", "Test Brewery 2" }
            };

            _breweryServiceMock
                .Setup(s => s.GetAutocompleteSuggestionsAsync(It.IsAny<BreweryFilter>()))
                .ReturnsAsync(suggestions);

            // Act
            var result = await _controller.GetAutocomplete(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<Dictionary<string, string>>(okResult.Value);
            Assert.Equal(2, returned.Count);
            Assert.Equal("Test Brewery 1", returned["1"]);
        }

        [Fact]
        public async Task GetAutocomplete_Returns500_OnException()
        {
            // Arrange
            var filter = new BreweryFilterDTO();
            _breweryServiceMock
                .Setup(s => s.GetAutocompleteSuggestionsAsync(It.IsAny<BreweryFilter>()))
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetAutocomplete(filter);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Internal server error", statusResult.Value);
        }
    }
}
