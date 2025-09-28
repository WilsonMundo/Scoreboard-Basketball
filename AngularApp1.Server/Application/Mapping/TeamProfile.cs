using AngularApp1.Server.Application.DTO.Teams;
using AngularApp1.Server.Domain.Model;
using AutoMapper;

namespace AngularApp1.Server.Application.Mapping
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<TeamCatalog, TeamDto>();
            CreateMap<TeamCreateDto, TeamCatalog>();
            CreateMap<TeamUpdateDto, TeamCatalog>();
        }
    }
}
