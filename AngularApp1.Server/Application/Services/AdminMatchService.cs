using AngularApp1.Server.Application.DTO;
using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using AutoMapper;
using static AngularApp1.Server.Application.DTO.Matches.Match;

namespace AngularApp1.Server.Application.Services
{
    public class AdminMatchService : IAdminMatchService
    {
        private readonly ITeamCatalogRepository _teams;
        private readonly IPlayerRepository _players;
        private readonly IGameAdminRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminMatchService> _logger;


        public AdminMatchService(ITeamCatalogRepository teams, IPlayerRepository players, IGameAdminRepository repo, IMapper mapper, ILogger<AdminMatchService> logger)
        { _teams = teams; _players = players; _repo = repo; _mapper = mapper; _logger = logger; }


        public async Task<ResultAPI<GameDto>> CreateFromCatalogAsync(CreateMatchDto dto)
        {
            var result = new ResultAPI<GameDto>(true);
            try
            {
                if (dto.HomeTeamId == dto.AwayTeamId) { result.Message = "Los equipos deben ser distintos"; result.Code = StatusHttpResponse.BadRequest; return result; }
                var home = await _teams.GetByIdAsync(dto.HomeTeamId); if (home is null) { result.Message = "HomeTeam no existe"; result.Code = StatusHttpResponse.BadRequest; return result; }
                var away = await _teams.GetByIdAsync(dto.AwayTeamId); if (away is null) { result.Message = "AwayTeam no existe"; result.Code = StatusHttpResponse.BadRequest; return result; }
                if (dto.QuarterSeconds <= 0) { result.Message = "QuarterSeconds debe ser > 0"; result.Code = StatusHttpResponse.BadRequest; return result; }


                var game = new Game { QuarterSeconds = dto.QuarterSeconds, RemainingSeconds = dto.QuarterSeconds, CreatedAtUtc = dto.StartAtUtc ?? DateTime.UtcNow };


                // Mantenemos compatibilidad con el tablero actual creando tambien los Team "embebidos"
                var teamHomeInGame = new Team { Name = home.Name, IsHome = true };
                var teamAwayInGame = new Team { Name = away.Name, IsHome = false };
                game.Teams.Add(teamHomeInGame);
                game.Teams.Add(teamAwayInGame);


                // Scores iniciales por Q1
                game.QuarterScores.Add(new QuarterScore { Game = game, Team = teamHomeInGame, QuarterNumber = 1, Points = 0 });
                game.QuarterScores.Add(new QuarterScore { Game = game, Team = teamAwayInGame, QuarterNumber = 1, Points = 0 });


                await _repo.AddGameAsync(game);
                await _repo.SaveChangesAsync();


                // (Opcional) Persistimos relación a catálogo para trazabilidad futura
                await _repo.AddGameTeamAsync(new GameTeam { GameId = game.Id, TeamId = home.Id, IsHome = true });
                await _repo.AddGameTeamAsync(new GameTeam { GameId = game.Id, TeamId = away.Id, IsHome = false });
                await _repo.SaveChangesAsync();


                var dtoOut = _mapper.Map<GameDto>(game);
                result.OkData(StatusHttpResponse.Created, dtoOut);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear partido desde catálogo");
                result.Message = "Error al crear partido"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }

        public async Task<ResultAPI<GameDto>> AssignRosterAsync(long gameId, AssignRosterDto dto)
        {
            var result = new ResultAPI<GameDto>(true);
            try
            {
                var game = await _repo.GetGameAsync(gameId);
                if (game is null) { result.Message = "Partido no encontrado"; result.Code = StatusHttpResponse.NotFound; return result; }


                // Debe existir relación GameTeam con el TeamId de catálogo
                if (!await _repo.ExistsGameTeamAsync(gameId, dto.TeamId))
                { result.Message = "El equipo no pertenece a este partido"; result.Code = StatusHttpResponse.BadRequest; return result; }


                // Limpiar roster anterior
                var prev = await _repo.GetRosterAsync(gameId, dto.TeamId);
                if (prev.Count > 0) await _repo.DeleteRosterAsync(prev);


                // Validar jugadores
                var roster = new List<GameRoster>(dto.PlayerIds?.Count ?? 0);
                if (dto.PlayerIds is not null)
                {
                    foreach (var pid in dto.PlayerIds.Distinct())
                    {
                        var p = await _players.GetByIdAsync(pid);
                        if (p is null || p.TeamId != dto.TeamId) { result.Message = $"Jugador {pid} no pertenece al equipo"; result.Code = StatusHttpResponse.BadRequest; return result; }
                        roster.Add(new GameRoster { GameId = gameId, TeamId = dto.TeamId, PlayerId = pid });
                    }
                }


                await _repo.AddRosterRangeAsync(roster);
                await _repo.SaveChangesAsync();


                // devolvemos estado del juego (para UI)
                var dtoOut = _mapper.Map<GameDto>(game);
                result.OkData(StatusHttpResponse.OK, dtoOut);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar roster {GameId}/{TeamId}", gameId, dto.TeamId);
                result.Message = "Error al asignar roster"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }

        public async Task<ResultAPI<IReadOnlyList<MatchHistoryItemDto>>> GetHistoryAsync()
        {
            var result = new ResultAPI<IReadOnlyList<MatchHistoryItemDto>>(true);
            try
            {
                var games = await _repo.GetGamesAsync();
                var items = games.Select(g => new MatchHistoryItemDto
                {
                    Id = g.Id,
                    Status = g.Status,
                    CreatedAtUtc = g.CreatedAtUtc,
                    HomeName = g.Teams.FirstOrDefault(t => t.IsHome)?.Name,
                    AwayName = g.Teams.FirstOrDefault(t => !t.IsHome)?.Name,
                    HomeScore = g.Teams.FirstOrDefault(t => t.IsHome)?.Score ?? 0,
                    AwayScore = g.Teams.FirstOrDefault(t => !t.IsHome)?.Score ?? 0
                }).ToList();
                result.OkData(StatusHttpResponse.OK, items);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de partidos");
                result.Message = "Error al obtener historial"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }
    }
}
