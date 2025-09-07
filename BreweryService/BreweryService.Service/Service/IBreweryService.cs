using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreweryService.Data.Entity;
using BreweryService.Data.Helper;

namespace BreweryService.Service
{
    public interface IBreweryService
    {
        Task<IEnumerable<Brewery>> GetBreweriesAsync(BreweryFilter breweryFilter);
        Task<Dictionary<string, string>> GetAutocompleteSuggestionsAsync(BreweryFilter breweryFilter);
    }
}
