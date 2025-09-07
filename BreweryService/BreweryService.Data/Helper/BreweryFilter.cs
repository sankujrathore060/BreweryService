using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreweryService.Data.Helper
{
    public class BreweryFilter
    {
        public BreweryFilter() {
            SortConfig = new List<Sorting>();
            Location = new Location();
        }
        public List<Sorting> SortConfig { get; set; }
        public Location? Location { get; set; }
        public string? SearchKey { get; set; }
        public string? SearchValue { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; } = 10;
    }

}
