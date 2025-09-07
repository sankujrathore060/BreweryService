using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreweryService.Service.Domain
{
    public class Pagging
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; }
    }
}
