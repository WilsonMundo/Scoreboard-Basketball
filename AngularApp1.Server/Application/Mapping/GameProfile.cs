using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Domain.Model;
using AutoMapper;

namespace AngularApp1.Server.Application.Mapping
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Game, GameDto>();
        }
    }
}
