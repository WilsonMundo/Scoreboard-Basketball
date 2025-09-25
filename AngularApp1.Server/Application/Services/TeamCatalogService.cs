using AngularApp1.Server.Application.DTO;
using AngularApp1.Server.Application.DTO.Teams;
using AngularApp1.Server.Application.Interfaces;
using AngularApp1.Server.Domain.Interface;
using AngularApp1.Server.Domain.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Application.Services
{
    public class TeamCatalogService : ITeamCatalogService
    {
        private readonly ITeamCatalogRepository _repo;
        private readonly IImageService _images;
        private readonly IMapper _mapper;
        private readonly ILogger<TeamCatalogService> _logger;
        public TeamCatalogService(ITeamCatalogRepository repo, IImageService images, IMapper mapper, ILogger<TeamCatalogService> logger)
        {
            _repo = repo; _images = images; _mapper = mapper; _logger = logger;
        }

        public async Task<ResultAPI<TeamDto>> CreateAsync(TeamCreateDto dto)
        {
            var result = new ResultAPI<TeamDto>(true);
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    result.Message = "El nombre es requerido";
                    result.Code = StatusHttpResponse.BadRequest; return result;
                }
                if (await _repo.ExistsByNameAsync(dto.Name))
                {
                    result.Message = "Ya existe un equipo con ese nombre";
                    result.Code = StatusHttpResponse.Conflict; return result;
                }
                var entity = new TeamCatalog { Name = dto.Name.Trim(), City = dto.City?.Trim() };
                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();


                var dtoOut = _mapper.Map<TeamDto>(entity);
                result.OkData(StatusHttpResponse.Created, dtoOut);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear equipo");
                result.Message = "Error al crear equipo";
                result.Code = StatusHttpResponse.InternalServerError;
                return result;
            }
        }
        public async Task<ResultAPI<TeamDto>> UpdateAsync(long id, TeamUpdateDto dto)
        {
            var result = new ResultAPI<TeamDto>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null)
                {
                    result.Message = "Equipo no encontrado";
                    result.Code = StatusHttpResponse.NotFound; return result;
                }
                if (!string.IsNullOrWhiteSpace(dto.Name) && await _repo.ExistsByNameAsync(dto.Name, id))
                {
                    result.Message = "Ya existe un equipo con ese nombre";
                    result.Code = StatusHttpResponse.Conflict; return result;
                }
                entity.Name = dto.Name.Trim();
                entity.City = dto.City?.Trim();
                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();


                var dtoOut = _mapper.Map<TeamDto>(entity);
                result.OkData(StatusHttpResponse.OK, dtoOut);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar equipo {TeamId}", id);
                result.Message = "Error al actualizar equipo";
                result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }
        public async Task<ResultAPI<object>> DeleteAsync(long id)
        {
            var result = new ResultAPI<object>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null)
                {
                    result.Message = "Equipo no encontrado";
                    result.Code = StatusHttpResponse.NotFound; return result;
                }
                await _repo.DeleteAsync(entity);
                await _repo.SaveChangesAsync();
                result.Ok(StatusHttpResponse.NoContent);
                return result;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Conflicto al eliminar equipo {TeamId}", id);
                result.Message = "No se puede eliminar: el equipo tiene relaciones";
                result.Code = StatusHttpResponse.Conflict; return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar equipo {TeamId}", id);
                result.Message = "Error al eliminar equipo";
                result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }
        public async Task<ResultAPI<TeamDto>> GetByIdAsync(long id)
        {
            var result = new ResultAPI<TeamDto>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null)
                {
                    result.Message = "Equipo no encontrado";
                    result.Code = StatusHttpResponse.NotFound; return result;
                }
                result.OkData(StatusHttpResponse.OK, _mapper.Map<TeamDto>(entity));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipo {TeamId}", id);
                result.Message = "Error al obtener equipo";
                result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }
        public async Task<ResultAPI<PagedResult<TeamDto>>> ListAsync(TeamListRequest req)
        {
            var result = new ResultAPI<PagedResult<TeamDto>>(true);
            try
            {
                var (items, total) = await _repo.ListAsync(req.Q, req.Page, req.Size, req.Sort);
                var dtoItems = items.Select(_mapper.Map<TeamDto>).ToList();
                var page = new PagedResult<TeamDto> { Page = Math.Max(1, req.Page), Size = Math.Clamp(req.Size, 1, 100), Total = total, Items = dtoItems };
                result.OkData(StatusHttpResponse.OK, page);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar equipos");
                result.Message = "Error al listar equipos";
                result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }
        public async Task<ResultAPI<TeamDto>> UploadLogoAsync(long id, IFormFile file)
        {
            var result = new ResultAPI<TeamDto>(true);
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity is null)
                {
                    result.Message = "Equipo no encontrado";
                    result.Code = StatusHttpResponse.NotFound; return result;
                }
                var url = await _images.SaveCompressedAsync(file, "team-logos", $"team_{id}");
                entity.LogoUrl = url;
                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();


                result.OkData(StatusHttpResponse.OK, _mapper.Map<TeamDto>(entity));
                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Archivo inválido para logo de equipo {TeamId}", id);
                result.Message = ex.Message;
                result.Code = StatusHttpResponse.BadRequest; return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir logo del equipo {TeamId}", id);
                result.Message = "Error al subir logo del equipo";
                result.Code = StatusHttpResponse.InternalServerError; return result;
            }
        }

    }
}
