using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.DTO.Score;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using AutoMapper;

namespace AngularApp1.Server.Application.Services
{
    public partial class GameService : IGameService
    {
        private readonly IGameRepository _repo;
        private readonly IMapper _mapper;
        private readonly IImageService _images;
        public GameService(IGameRepository repo, IMapper mapper, IImageService images)
        {
            _repo = repo; _mapper = mapper;
            _images = images;
        }


        public async Task<GameDto> CreateAsync(CreateGameDto dto)
        {
            if (dto.QuarterSeconds <= 0) throw new ArgumentException("Los cuartos de segundo deben ser > 0");            
            var game = new Game { QuarterSeconds = dto.QuarterSeconds };
            game.Teams.Add(new Team { Name = dto.HomeName, IsHome = true });
            game.Teams.Add(new Team { Name = dto.AwayName, IsHome = false });
            
            
            

            var home = game.Teams.First(t => t.IsHome);
            var away = game.Teams.First(t => !t.IsHome);

            game.RemainingSeconds = game.QuarterSeconds;

            game.QuarterScores.Add(new QuarterScore { Game = game, Team = home, QuarterNumber = 1, Points = 0 });
            game.QuarterScores.Add(new QuarterScore { Game = game, Team = away, QuarterNumber = 1, Points = 0 });

            await _repo.AddAsync(game);
            await _repo.SaveChangesAsync();



    

            return _mapper.Map<GameDto>(game);
        }


        public async Task<GameDto?> GetAsync(long id)
        {
            var game = await _repo.GetAsync(id);
            return game is null ? null : _mapper.Map<GameDto>(game);
        }
        public async Task<GameDto> UpdateScoreAsync(long gameId, UpdateScoreDto dto)
        {
            if (dto.DeltaPoints is < -1 or > 3 || dto.DeltaPoints == 0)
                throw new ArgumentException("DeltaPoints permitido: -1, +1, +2, +3");

            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");
            var team = game.Teams.FirstOrDefault(t => t.Id == dto.TeamId)
                   ?? throw new KeyNotFoundException($"Team {dto.TeamId} no pertenece al juego {gameId}");

            // QuarterScore del cuarto actual (crea si no existe)
            var qs = game.QuarterScores.FirstOrDefault(x => x.TeamId == dto.TeamId && x.QuarterNumber == game.Quarter);
            if (qs is null)
            {
                qs = new QuarterScore { GameId = game.Id, TeamId = team.Id, QuarterNumber = game.Quarter, Points = 0, IsOvertime = game.Quarter > 4 };
                game.QuarterScores.Add(qs);
            }

            // aplica delta con clamp
            var newQuarterPts = Math.Max(0, qs.Points + dto.DeltaPoints);
            var deltaApplied = newQuarterPts - qs.Points; // por si clamp evita negativos
            qs.Points = newQuarterPts;

            // cache total del equipo
            var newTeamScore = Math.Max(0, team.Score + deltaApplied);
            team.Score = newTeamScore;

            await _repo.SaveChangesAsync();
            return _mapper.Map<GameDto>(game);
        }
        public async Task<GameDto> UploadTeamLogoAsync(long gameId, long teamId, IFormFile file)
        {
            var game = await _repo.GetAsync(gameId); 
            if (game is null) throw new KeyNotFoundException($"Game {gameId} no existe");

            var team = game.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team is null) throw new KeyNotFoundException($"Team {teamId} no pertenece al juego {gameId}");

            var url = await _images.SaveCompressedAsync(file, "logos", $"game{gameId}_team{teamId}");
            team.LogoUrl = url;

            await _repo.SaveChangesAsync();
            return _mapper.Map<GameDto>(game);
        }
    }

}
