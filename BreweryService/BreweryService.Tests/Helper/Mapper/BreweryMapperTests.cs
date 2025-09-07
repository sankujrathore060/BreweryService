using BreweryService.Data.Entity;
using BreweryService.DTO;
using BreweryService.Helper.Mapper;

namespace BreweryService.Tests.Helper.Mapper
{
    public class BreweryMapperTests
    {
        [Fact]
        public void Brewery_ToModel_MapsCorrectly()
        {
            // Arrange
            var brewery = new Brewery
            {
                Id = "1",
                Name = "Test Brewery",
                City = "Test City",
                Country = "Test Country",
                Phone = "1234567890"
            };

            // Act
            var dto = brewery.ToModel();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(brewery.Id, dto.Id);
            Assert.Equal(brewery.Name, dto.Name);
            Assert.Equal(brewery.City, dto.City);
            Assert.Equal(brewery.Phone, dto.Phone);
        }

        [Fact]
        public void BreweryList_ToModel_MapsCorrectly()
        {
            // Arrange
            var breweries = new List<Brewery>
            {
                new Brewery { Id = "1", Name = "Brewery 1", City = "City 1" },
                new Brewery { Id = "2", Name = "Brewery 2", City = "City 2" }
            };

            // Act
            var dtos = breweries.ToModel();

            // Assert
            Assert.NotNull(dtos);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Brewery 1", dtos[0].Name);
            Assert.Equal("Brewery 2", dtos[1].Name);
        }

        [Fact]
        public void BreweryDTO_ToEntity_MapsCorrectly()
        {
            // Arrange
            var dto = new BreweryDTO
            {
                Id = "10",
                Name = "DTO Brewery",
                City = "DTO City",
                Phone = "9876543210"
            };

            // Act
            var entity = dto.ToEntity("ignoredKey"); // key param unused in mapping

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.Name, entity.Name);
            Assert.Equal(dto.City, entity.City);
            Assert.Equal(dto.Phone, entity.Phone);
        }
    }
}
