using System.Text.Json;
using BreweryService.Common;
using BreweryService.Data.Entity;
using BreweryService.Data.Helper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BreweryService.Data.Repository
{
    public class BreweryRepository : IBreweryRepository
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly BreweryServiceAppSettings _breweryServiceAppSettings;
        private readonly ILogger<IBreweryRepository> _logger;

        public BreweryRepository(IMemoryCache cache, HttpClient httpClient, IOptions<BreweryServiceAppSettings> breweryServiceAppSettings, ILogger<IBreweryRepository> logger)
        {
            _cache = cache;
            _httpClient = httpClient;
            _breweryServiceAppSettings = breweryServiceAppSettings.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync(BreweryFilter breweryFilter)
        {
            try
            {
                string breweryDbURL = GetBreweryDbURL(breweryFilter, false);
                string cacheKey = _breweryServiceAppSettings.CacheKey + GetCacheKeyFromURL(breweryDbURL.ToLower());
                if (_cache.TryGetValue(cacheKey, out IEnumerable<Brewery> cachedBreweries))
                {
                    return cachedBreweries;
                }

                _logger.LogInformation("Fetching Data for Brewery");
                var response = await _httpClient.GetAsync(breweryDbURL);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                IEnumerable<Brewery> breweries = new List<Brewery>();

                if (json.TrimStart().StartsWith("["))
                {
                    breweries = JsonSerializer.Deserialize<List<Brewery>>(json);
                }

                _logger.LogInformation("Caching Data for Brewery");
                await CacheBreweriesAsync(cacheKey, breweries);
                return breweries;
            }
            catch (Exception ex) {
                _logger.LogError("Internal Service Error from Brewery Repository");
                throw new Exception("Internal Server Error");
            }
        }

        private string GetCacheKeyFromURL(String breweryDbURL)
        {
            if(breweryDbURL.Contains("/autocomplete") && breweryDbURL.Split("/autocomplete").Length > 1) return "/autocomplete" + breweryDbURL.Split("/autocomplete")[1];
            if (breweryDbURL.Contains("/search") && breweryDbURL.Split("/search").Length > 1) return "/search" + breweryDbURL.Split("/search")[1];
            if (breweryDbURL.Contains("?") && breweryDbURL.Split("?").Length > 1) return breweryDbURL.Split("?")[1];
            return string.Empty;
        }

        private string GetBreweryDbURL(BreweryFilter breweryFilter, bool isAutoComplete)
        {
            string breweryDbURL = _breweryServiceAppSettings.OpenbrewerydbURL;
            if (isAutoComplete) breweryDbURL += "/autocomplete";
            //Search Query 
            if (string.IsNullOrEmpty(breweryFilter.SearchKey) && !string.IsNullOrEmpty(breweryFilter.SearchValue))
            {
                breweryDbURL += "/search?query=" + breweryFilter.SearchValue + "&";
            }
            else
            {
                breweryDbURL += "?";
            }
            //Paggination
            if (!breweryFilter.PageSize.HasValue || breweryFilter.PageSize == 0)
            {
                breweryDbURL += "per_page=50";
            }
            else if (breweryFilter.PageSize >= 200)
            {
                breweryDbURL += "per_page=200";
            }
            else
            {
                breweryDbURL += "per_page=" + breweryFilter.PageSize;
            }

            if(breweryFilter.Page.HasValue && breweryFilter.Page != 0) {
                breweryDbURL+= "&page=" + breweryFilter.Page;
            }

            //Sorting Config
            if (breweryFilter.SortConfig != null) {
                string sortConfig = string.Empty;
                foreach(var item in breweryFilter.SortConfig){
                    if (string.IsNullOrEmpty(sortConfig))
                    {
                        sortConfig += "sort=";
                    }
                    else {
                        sortConfig += ",";
                    }
                    sortConfig += item.SortColumn + (string.IsNullOrEmpty(item.SortOrder) ? string.Empty :  ":" + item.SortOrder);
                }

                breweryDbURL += "&" + sortConfig;
            }

            if (breweryFilter.Location != null && breweryFilter.Location.Latitude != null && breweryFilter.Location.Longitude != null) {
                breweryDbURL += "&by_dist=" + breweryFilter.Location.Latitude + "," + breweryFilter.Location.Longitude;
            }

            if (!string.IsNullOrEmpty(breweryFilter.SearchKey) && !string.IsNullOrEmpty(breweryFilter.SearchValue)) {
                List<string> searchKeys = new List<string>() { "ids", "name", "state", "postal", "type" };

                if (searchKeys.Contains(breweryFilter.SearchKey))
                {
                    breweryDbURL += "&by_" + breweryFilter.SearchKey + "=" + breweryFilter.SearchValue;
                }
            }

            return breweryDbURL;
        }

        private Task CacheBreweriesAsync(String cacheKey, IEnumerable<Brewery> breweries)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, breweries, cacheOptions);
            return Task.CompletedTask;
        }

    }
}
