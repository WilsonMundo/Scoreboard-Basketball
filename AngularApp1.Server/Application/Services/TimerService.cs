using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace AngularApp1.Server.Application.Services
{
    public partial class GameService : IGameService
    {
        public async Task<GameDto> StartTimerAsync(long gameId)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");

            
            if (!game.IsTimerRunning && game.RemainingSeconds > 0)
            {
                game.IsTimerRunning = true;
                game.TimerStartedAtUtc = DateTime.UtcNow;
                await _repo.SaveChangesAsync();
            }

            var dtoGame = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGame);

            return dtoGame;
        }

        public async Task<GameDto> PauseTimerAsync(long gameId)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");
          
             if (game.IsTimerRunning && game.TimerStartedAtUtc.HasValue)
            {
                var elapsed = (int)Math.Floor((DateTime.UtcNow - game.TimerStartedAtUtc.Value).TotalSeconds);
                if (elapsed > 0)
                {
                    game.RemainingSeconds = Math.Max(0, game.RemainingSeconds - elapsed);
                }
                game.IsTimerRunning = false;
                game.TimerStartedAtUtc = null;
                await _repo.SaveChangesAsync();
            }

            var dtoGame = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGame);

            return dtoGame;
        }

        public async Task<GameDto> ResetTimerAsync(long gameId, int? newQuarterSeconds = null)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");

            if (newQuarterSeconds.HasValue && newQuarterSeconds.Value > 0)
            {
                game.QuarterSeconds = newQuarterSeconds.Value;
            }
            game.RemainingSeconds = game.QuarterSeconds;
            game.IsTimerRunning = false;
            game.TimerStartedAtUtc = null;

            await _repo.SaveChangesAsync();
            var dtoGame = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGame);

            return dtoGame; 
        }

        public async Task<GameDto> NextQuarterAsync(long gameId)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");



            if (game.Quarter < 4)
            {
                // Avanza de Q1→Q4 normal
                game.Quarter += 1;
            }else
            {

                int homeScore = game.Teams.First(t => t.IsHome).Score;
                int awayScore = game.Teams.First(t => !t.IsHome).Score;
                bool isTie = homeScore == awayScore;

                if (game.Quarter == 4)
                {
                    // Para pasar a OT1 debe haber empate
                    if (!isTie)
                    {
                        game.Status = "Finished";
                        game.IsTimerRunning = false; game.TimerStartedAtUtc = null;
                        var dtoGames = _mapper.Map<GameDto>(game);
                        await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGames);

                        return dtoGames;
                    }
                    game.Quarter = 5; // OT1
                }
                else // Quarter >= 5  (OTs)
                {
                    // Solo crear OT adicional si sigue empatado
                    if (!isTie)
                        throw new InvalidOperationException("Ya hay un ganador; no se puede crear más overtime.");
                    game.Quarter += 1; // OT2, OT3, ...
                }
            }


            



            
            game.IsTimerRunning = false;
            game.TimerStartedAtUtc = null;
            game.RemainingSeconds = game.QuarterSeconds;




            // asegurar QuarterScores para el nuevo cuarto
            foreach (var t in game.Teams)
            {
                if (!game.QuarterScores.Any(q => q.TeamId == t.Id && q.QuarterNumber == game.Quarter))
                {
                    game.QuarterScores.Add(new QuarterScore
                    {
                        GameId = game.Id,
                        TeamId = t.Id,
                        QuarterNumber = game.Quarter,
                        Points = 0,
                        IsOvertime = game.Quarter > 4
                    });
                }
            }

            await _repo.SaveChangesAsync();
            var dtoGame = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGame);

            return dtoGame;
        }

        public async Task<GameDto> PrevQuarterAsync(long gameId)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");
            if (game.Quarter > 1) game.Quarter -= 1;

            game.IsTimerRunning = false;
            game.TimerStartedAtUtc = null;
            game.RemainingSeconds = game.QuarterSeconds;

            // (no cambiamos puntajes)
            await _repo.SaveChangesAsync();
            
            var dtoGame = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGame);

            return dtoGame; 
        }
        public async Task<GameDto> FinishAsync(long gameId)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");

            if (game.Quarter < 4)
                throw new InvalidOperationException("Solo se puede finalizar a partir del 4º cuarto.");

            var home = game.Teams.First(t => t.IsHome).Score;
            var away = game.Teams.First(t => !t.IsHome).Score;

            if (home == away)
                throw new InvalidOperationException("No se puede finalizar con empate. Usa overtime.");

            game.Status = "Finished";
            game.IsTimerRunning = false;
            game.TimerStartedAtUtc = null;

            await _repo.SaveChangesAsync();
            var dtoGame = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dtoGame);

            return dtoGame; ;
        }
        private void MaterializeTimer(Game game, DateTime nowUtc)
        {
            if (game.IsTimerRunning && game.TimerStartedAtUtc.HasValue)
            {
                var elapsed = (int)Math.Floor((nowUtc - game.TimerStartedAtUtc.Value).TotalSeconds);
                if (elapsed > 0)
                {
                    // Trae el estado al “ahora”
                    game.RemainingSeconds = Math.Max(0, game.RemainingSeconds - elapsed);
                    game.TimerStartedAtUtc = nowUtc; // reancla para no volver a descontar lo mismo
                    if (game.RemainingSeconds == 0) { game.IsTimerRunning = false; }
                }
            }
        }
        public async Task<GameDto> UpdateFoulsAsync(long gameId, long teamId, int delta)
        {
            var game = await _repo.GetAsync(gameId) ?? throw new KeyNotFoundException($"Game {gameId} no existe");
            var team = game.Teams.FirstOrDefault(t => t.Id == teamId)
                   ?? throw new KeyNotFoundException($"Team {teamId} no pertenece al juego {gameId}");

            
            team.Fouls = Math.Max(0, team.Fouls + delta);

            await _repo.SaveChangesAsync();

            // materializar antes de devolver/emitar
            MaterializeTimer(game, DateTime.UtcNow);
            var dto = _mapper.Map<GameDto>(game);
            await _hub.Clients.Group($"game:{game.Id}").SendAsync("GameUpdated", dto);
            return dto;
        }

        public async Task<IReadOnlyList<GameListItemDto>> GetAllAsync()
        {
            var games = await _repo.GetAll();
                           

            return games.Select(g =>
            {
                var home = g.Teams.FirstOrDefault(t => t.IsHome);
                var away = g.Teams.FirstOrDefault(t => !t.IsHome);
                return new GameListItemDto
                {
                    Id = g.Id,
                    Status = g.Status,
                    Quarter = g.Quarter,
                    HomeName = home?.Name,
                    AwayName = away?.Name,
                    HomeScore = home?.Score ?? 0,
                    AwayScore = away?.Score ?? 0,
                    CreatedAtUtc = g.CreatedAtUtc
                };
            }).ToList();
        }
    }
}
