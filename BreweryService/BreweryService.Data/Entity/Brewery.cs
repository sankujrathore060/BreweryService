using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BreweryService.Data.Entity
{
    public class Brewery : BaseEntity
    {
        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [JsonPropertyName("brewery_type")] 
        public string BreweryType { get; set; }

        [JsonPropertyName("address_1")] 
        public string Address1 { get; set; }

        [JsonPropertyName("address_2")] 
        public string Address2 { get; set; }

        [JsonPropertyName("address_3")] 
        public string Address3 { get; set; }

        [JsonPropertyName("street")] 
        public string Street { get; set; }

        [JsonPropertyName("city")] 
        public string City { get; set; }

        [JsonPropertyName("state_province")] 
        public string StateProvince { get; set; }

        [JsonPropertyName("state")] 
        public string State { get; set; }

        [JsonPropertyName("postal_code")] 
        public string PostalCode { get; set; }

        [JsonPropertyName("country")] 
        public string Country { get; set; }

        [JsonPropertyName("longitude")] 
        public double? Longitude { get; set; }

        [JsonPropertyName("latitude")] 
        public double? Latitude { get; set; }

        [JsonPropertyName("phone")] 
        public string Phone { get; set; }
        [JsonPropertyName("website_url")] 
        public string WebsiteUrl { get; set; }
    }
}
