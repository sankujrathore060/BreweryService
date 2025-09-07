using AutoMapper;
using BreweryService.Data.Entity;
using BreweryService.DTO;

namespace BreweryService.Helper.Mapper
{
    public static partial class MappingExtensions
    {
        public static IList<BreweryDTO> ToModel(this IList<Brewery> entity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Brewery, BreweryDTO>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper.Map<IList<Brewery>, IList<BreweryDTO>>(entity);
        }

        public static BreweryDTO ToModel(this Brewery entity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Brewery, BreweryDTO>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<Brewery, BreweryDTO>(entity);
        }
        public static Brewery ToEntity(this BreweryDTO model, string key)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BreweryDTO, Brewery>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<BreweryDTO, Brewery>(model);
        }

    }
}
