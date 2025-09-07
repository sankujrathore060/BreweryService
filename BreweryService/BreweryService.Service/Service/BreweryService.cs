using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreweryService.Data.Entity;
using BreweryService.Data.Repository;
using BreweryService.Data.Helper;
using Microsoft.Extensions.Logging;

namespace BreweryService.Service
{
    public class BreweryService : IBreweryService
    {
        private readonly IBreweryRepository _repository;
        private readonly ILogger<IBreweryService> _logger;

        public BreweryService(IBreweryRepository repository, ILogger<IBreweryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Brewery>> GetBreweriesAsync(BreweryFilter breweryFilter)
        {
            _logger.LogInformation("Fetching data  for Breweries from service");
            return await _repository.GetAllBreweriesAsync(breweryFilter);
        }

        

        public async Task<Dictionary<string, string>> GetAutocompleteSuggestionsAsync(BreweryFilter breweryFilter)
        {

            _logger.LogInformation("Fetching Autocomplete Suggestion for Breweries");
            var breweries = await _repository.GetAllBreweriesAsync(breweryFilter);
            return breweries
                .Select(b => new KeyValuePair<string, string>(b.Id, b.Name))
                .Distinct()
                .Take(10).ToDictionary();
        }

    }
}
