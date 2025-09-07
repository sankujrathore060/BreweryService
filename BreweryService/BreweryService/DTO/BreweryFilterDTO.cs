using BreweryService.Data.Helper;

namespace BreweryService.DTO
{
    public class BreweryFilterDTO
    {
        public BreweryFilterDTO()
        {
            SortConfig = new List<Sorting>();
            Location = new Location();
        }
        public List<Sorting>? SortConfig { get; set; }
        public Location? Location { get; set; }
        public string? SearchKey { get; set; }
        public string? SearchValue { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; } = 10;
    }
}
