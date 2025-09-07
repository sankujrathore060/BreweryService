using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreweryService.Data.Entity;
using BreweryService.Data.Helper;

namespace BreweryService.Data.Repository
{
    public interface IBreweryRepository
    {
        Task<IEnumerable<Brewery>> GetAllBreweriesAsync(BreweryFilter breweryFilter);
    }
}
