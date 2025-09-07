using Asp.Versioning;
using BreweryService.Service;
using Microsoft.AspNetCore.Mvc;
using BreweryService.Helper.Mapper;
using BreweryService.DTO;

namespace BreweryService.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BreweriesController : ControllerBase
    {
        private readonly IBreweryService _breweryService;
        private readonly ILogger<BreweriesController> _logger;

        public BreweriesController(IBreweryService breweryService, ILogger<BreweriesController> logger)
        {
            _breweryService = breweryService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] BreweryFilterDTO filterBox)
        {
            try
            {
                var breweries = await _breweryService.GetBreweriesAsync(filterBox.ToEntity());
                return Ok(breweries.ToList().ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching breweries");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("autocomplete")]
        public async Task<IActionResult> GetAutocomplete([FromBody] BreweryFilterDTO filterBox)
        {
            try
            {
                var suggestions = await _breweryService.GetAutocompleteSuggestionsAsync(filterBox.ToEntity());
                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in autocomplete");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
