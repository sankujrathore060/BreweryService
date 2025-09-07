using BreweryService.DTO;
using BreweryService.Data.Helper;
using BreweryService.Helper.Mapper;

namespace BreweryService.Tests.Helper.Mapper
{
    public class BreweryFilterMappingTests
    {
        [Fact]
        public void BreweryFilterDTO_ToEntity_MapsCorrectly()
        {
            // Arrange
            var dto = new BreweryFilterDTO
            {
                SearchKey = "name",
                SearchValue = "brewery",
                Page = 2,
                SortConfig = new List<Sorting>
                {
                    new Sorting { SortColumn = "Name", SortOrder = "asc" },
                    new Sorting { SortColumn = "City", SortOrder = "desc" }
                },
                Location = new Location
                {
                   Latitude = 12.003,
                   Longitude = 20.003
                }
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(dto.SearchKey, entity.SearchKey);
            Assert.Equal(dto.SearchValue, entity.SearchValue);
            Assert.Equal(dto.Page, entity.Page);

            Assert.NotNull(entity.SortConfig);
            Assert.Equal(2, entity.SortConfig.Count);

            Assert.NotNull(entity.Location);
        }
    }
}
