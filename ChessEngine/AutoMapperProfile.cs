using AutoMapper;
using ChessEngineLogic;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngineLogic
{
    public static class AutoMapperProfile
    {
        public static Mapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Player, PlayerModel>();
                cfg.CreateMap<PlayerModel, Player>();
                cfg.CreateMap<Game, GameModel>();
                cfg.CreateMap<GameModel, Game>();
                
            });

            return new AutoMapper.Mapper(config);
        }
    }
}
