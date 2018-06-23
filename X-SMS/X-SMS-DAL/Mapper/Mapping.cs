using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X_SMS_DAL.Database;
using X_SMS_REP;

namespace X_SMS_DAL.Mapper
{
    public static class Mapping
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Game, GameDTO>();
            CreateMap<GameDTO,Game>();
            CreateMap<Player, PlayerDTO>();
            CreateMap<PlayerDTO, Player>();
            CreateMap<List<Game>, List<GameDTO>>();
            CreateMap< List<ViewPlayerPortfolio>, List<PlayerPortfolioDTO>>();
            CreateMap<BankAccount, BankAccountDTO>();
            // Additional mappings here...
        }
    }

}
