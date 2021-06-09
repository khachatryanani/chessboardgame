using AutoMapper;
using ChessEngineLogic;
using System;
using System.Collections.Generic;
using System.Text;

namespace KingdomChessGame_Desktop
{
    public static class AutoMapperProfile
    {
        public static Mapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlayerModel, PlayerViewModel>();
                cfg.CreateMap<PlayerViewModel, PlayerModel>();

                cfg.CreateMap<GameModel, GameViewModel>()
                                .ForPath(dest => dest.StartDate,
                                             opt => opt.MapFrom(src => src.StartDate.ToShortDateString()))
                                .ForPath(dest => dest.Result,
                                             opt => opt.MapFrom(src => src.Result == 0 ? "Paused" : src.Result == 1 ? "Mate" : "StaleMate"))
                                .ForPath(dest => dest.Winner,
                                             opt => opt.MapFrom(src => src.Result == 1 ? src.Turn ? src.Black.Name : src.White.Name : "N/A"))
                                .ForPath(dest => dest.Turn,
                                             opt => opt.MapFrom(src => src.Result != 0 ? "N/A" : src.Turn ? src.White.Name : src.Black.Name));
                

            });

            return new AutoMapper.Mapper(config);
        }
    }
}
