using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using AutoMapper;

namespace AngularApp1.Server.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repo;
        private readonly IMapper _mapper;
        public GameService(IGameRepository repo, IMapper mapper)
        {
            _repo = repo; _mapper = mapper;
        }


        public async Task<GameDto> CreateAsync(CreateGameDto dto)
        {
            if (dto.QuarterSeconds <= 0) throw new ArgumentException("Los cuartos de segundo deben ser > 0");
            var game = new Game { QuarterSeconds = dto.QuarterSeconds };
            await _repo.AddAsync(game);
            await _repo.SaveChangesAsync();
            return _mapper.Map<GameDto>(game);
        }


        public async Task<GameDto?> GetAsync(long id)
        {
            var game = await _repo.GetAsync(id);
            return game is null ? null : _mapper.Map<GameDto>(game);
        }
    }

}
