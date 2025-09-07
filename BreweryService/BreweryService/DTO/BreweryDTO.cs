using System.Text.Json.Serialization;

namespace BreweryService.DTO
{
    public class BreweryDTO : BaseDTO
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
    }
}
