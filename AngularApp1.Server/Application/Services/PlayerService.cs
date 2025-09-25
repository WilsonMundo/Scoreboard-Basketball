using AngularApp1.Server.Application.DTO;
using AngularApp1.Server.Application.DTO.Players;
using AngularApp1.Server.Application.DTO.Teams;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repo;
        private readonly ITeamCatalogRepository _teams;
        private readonly IMapper _mapper;
        private readonly ILogger<PlayerService> _logger;


        public PlayerService(IPlayerRepository repo, ITeamCatalogRepository teams, IMapper mapper, ILogger<PlayerService> logger)
        { _repo = repo; _teams = teams; _mapper = mapper; _logger = logger; }

        public async Task<ResultAPI<PlayerDto>> CreateAsync(PlayerCreateDto dto)
        {
            var result = new ResultAPI<PlayerDto>(true);
            try
            {
                if (string.IsNullOrWhiteSpace(dto.FullName)) { result.Message = "El nombre es requerido"; result.Code = StatusHttpResponse.BadRequest; return result; }
                if (dto.TeamId.HasValue)
                {
                    var team = await _teams.GetByIdAsync(dto.TeamId.Value);
                    if (team is null) { result.Message = "Equipo no existe"; result.Code = StatusHttpResponse.BadRequest; return result; }
                    if (dto.Number.HasValue && await _repo.ExistsSameNumberInTeamAsync(dto.TeamId.Value, dto.Number.Value))
                    { result.Message = "Número ya usado en el equipo"; result.Code = StatusHttpResponse.Conflict; return result; }
                }
                var entity = new Player
                {
                    FullName = dto.FullName.Trim(),
                    Number = dto.Number,
                    Position = dto.Position?.Trim(),
                    HeightMeters = dto.HeightMeters,
                    Age = dto.Age??0,
                    Nationality = dto.Nationality?.Trim(),
                    TeamId = dto.TeamId
                };
                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();
                result.OkData(StatusHttpResponse.Created, _mapper.Map<PlayerDto>(entity));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear jugador");
                result.Message = "Error al crear jugador"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }


        public async Task<ResultAPI<PlayerDto>> UpdateAsync(long id, PlayerUpdateDto dto)
        {
            var result = new ResultAPI<PlayerDto>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null) { result.Message = "Jugador no encontrado"; result.Code = StatusHttpResponse.NotFound; return result; }


                if (dto.TeamId.HasValue)
                {
                    var team = await _teams.GetByIdAsync(dto.TeamId.Value);
                    if (team is null) { result.Message = "Equipo no existe"; result.Code = StatusHttpResponse.BadRequest; return result; }
                    if (dto.Number.HasValue && await _repo.ExistsSameNumberInTeamAsync(dto.TeamId.Value, dto.Number.Value, id))
                    { result.Message = "Número ya usado en el equipo"; result.Code = StatusHttpResponse.Conflict; return result; }
                }


                entity.FullName = dto.FullName.Trim();
                entity.Number = dto.Number;
                entity.Position = dto.Position?.Trim();
                entity.HeightMeters = dto.HeightMeters;
                entity.Age = dto.Age?? 0;
                entity.Nationality = dto.Nationality?.Trim();
                entity.TeamId = dto.TeamId;


                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();
                result.OkData(StatusHttpResponse.OK, _mapper.Map<PlayerDto>(entity));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar jugador {PlayerId}", id);
                result.Message = "Error al actualizar jugador"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }

        public async Task<ResultAPI<object>> DeleteAsync(long id)
        {
            var result = new ResultAPI<object>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null) { result.Message = "Jugador no encontrado"; result.Code = StatusHttpResponse.NotFound; return result; }
                await _repo.DeleteAsync(entity);
                await _repo.SaveChangesAsync();
                result.Ok(StatusHttpResponse.NoContent);
                return result;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Conflicto al eliminar jugador {PlayerId}", id);
                result.Message = "No se puede eliminar: el jugador está relacionado con partidos"; result.Code = StatusHttpResponse.Conflict; return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar jugador {PlayerId}", id);
                result.Message = "Error al eliminar jugador"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }


        public async Task<ResultAPI<PlayerDto>> GetByIdAsync(long id)
        {
            var result = new ResultAPI<PlayerDto>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null) { result.Message = "Jugador no encontrado"; result.Code = StatusHttpResponse.NotFound; return result; }
                result.OkData(StatusHttpResponse.OK, _mapper.Map<PlayerDto>(entity));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jugador {PlayerId}", id);
                result.Message = "Error al obtener jugador"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }


        public async Task<ResultAPI<PagedResult<PlayerDto>>> ListAsync(PlayerListRequest req)
        {
            var result = new ResultAPI<PagedResult<PlayerDto>>(true);
            try
            {
                var (items, total) = await _repo.ListAsync(req.Q, req.TeamId, req.Page, req.Size, req.Sort);
                var dtos = items.Select(_mapper.Map<PlayerDto>).ToList();
                var page = new PagedResult<PlayerDto> { Page = Math.Max(1, req.Page), Size = Math.Clamp(req.Size, 1, 100), Total = total, Items = dtos };
                result.OkData(StatusHttpResponse.OK, page);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar jugadores");
                result.Message = "Error al listar jugadores"; result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }
    }
}
