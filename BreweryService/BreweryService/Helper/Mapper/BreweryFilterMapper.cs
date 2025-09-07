using AutoMapper;
using BreweryService.DTO;
using BreweryService.Data.Helper;

namespace BreweryService.Helper.Mapper
{
    public static partial class MappingExtensions
    {
        public static BreweryFilter ToEntity(this BreweryFilterDTO model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BreweryFilterDTO, BreweryFilter>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<BreweryFilterDTO, BreweryFilter>(model);
        }

    }
}
