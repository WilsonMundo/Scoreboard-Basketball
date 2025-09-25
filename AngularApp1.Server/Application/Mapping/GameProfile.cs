using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.DTO.Players;
using AngularApp1.Server.Domain.Model;
using AutoMapper;

namespace AngularApp1.Server.Application.Mapping
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Team, TeamDto>();            
            CreateMap<Game, GameDto>()
            .ForMember(d => d.Home, m => m.MapFrom(s => s.Teams.FirstOrDefault(t => t.IsHome)))
            .ForMember(d => d.Away, m => m.MapFrom(s => s.Teams.FirstOrDefault(t => !t.IsHome)))
            .ForMember(d => d.Box, m => m.MapFrom(s => BuildBox(s)));
            CreateMap<TeamCatalog, TeamDto>();
            CreateMap<Player, PlayerDto>()
                        .ForMember(d => d.TeamName, m => m.MapFrom(s => s.Team != null ? s.Team.Name : null));
        }
        private static IReadOnlyList<QuarterPointsDto> BuildBox(Game g)
        {
            var homeId = g.Teams.FirstOrDefault(t => t.IsHome)?.Id ?? 0;
            var awayId = g.Teams.FirstOrDefault(t => !t.IsHome)?.Id ?? 0;

            var byQuarter =
                g.QuarterScores
                 .GroupBy(q => q.QuarterNumber)
                 .OrderBy(grp => grp.Key)
                 .Select(grp =>
                 {
                     var h = grp.FirstOrDefault(x => x.TeamId == homeId)?.Points ?? 0;
                     var a = grp.FirstOrDefault(x => x.TeamId == awayId)?.Points ?? 0;
                     var anyOt = grp.Any(x => x.IsOvertime);
                     return new QuarterPointsDto { Quarter = grp.Key, Home = h, Away = a, IsOvertime = anyOt };
                 })
                 .ToList();

            return byQuarter;
        }
    }
}
