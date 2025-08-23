using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.DTO.Score;
using AngularApp1.Server.Application.DTO.Timer;
using AngularApp1.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _svc;
        public GamesController(IGameService svc) => _svc = svc;


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateGameDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtRoute("GetById", new { id = created.Id }, created);

        }


        [HttpGet("{id:long}", Name = "GetById")]
        public async Task<ActionResult<GameDto>> GetById(long id)
        {
            var game = await _svc.GetAsync(id);
            return game is null ? NotFound() : Ok(game);
        }
        [HttpPost("{id:long}/score")]
        public async Task<ActionResult<GameDto>> UpdateScore(long id, [FromBody] UpdateScoreDto dto)
        {
            try
            {
                var updated = await _svc.UpdateScoreAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
            catch (ArgumentException e) { return BadRequest(new { message = e.Message }); }
        }
        [HttpPost("{id:long}/teams/{teamId:long}/logo")]
        public async Task<ActionResult<GameDto>> UploadTeamLogo(long id, long teamId, IFormFile file)
        {
            try
            {
                var dto = await _svc.UploadTeamLogoAsync(id, teamId, file);
                return Ok(dto);
            }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
            catch (ArgumentException e) { return BadRequest(new { message = e.Message }); }
        }

        [HttpPost("{id:long}/timer/start")]
        public async Task<ActionResult<GameDto>> StartTimer(long id)
        {
            try { return Ok(await _svc.StartTimerAsync(id)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
        }

        [HttpPost("{id:long}/timer/pause")]
        public async Task<ActionResult<GameDto>> PauseTimer(long id)
        {
            try { return Ok(await _svc.PauseTimerAsync(id)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
        }

        [HttpPost("{id:long}/timer/reset")]
        public async Task<ActionResult<GameDto>> ResetTimer(long id, [FromBody] ResetTimerDto? body)
        {
            try { return Ok(await _svc.ResetTimerAsync(id, body?.QuarterSeconds)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
        }
        [HttpPost("{id:long}/quarter/next")]
        public async Task<ActionResult<GameDto>> NextQuarter(long id)
        {
            try { return Ok(await _svc.NextQuarterAsync(id)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
            catch (InvalidOperationException e) { return BadRequest(new { message = e.Message }); }
        }

        [HttpPost("{id:long}/quarter/prev")]
        public async Task<ActionResult<GameDto>> PrevQuarter(long id)
        {
            try { return Ok(await _svc.PrevQuarterAsync(id)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
        }
        [HttpPost("{id:long}/finish")]
        public async Task<ActionResult<GameDto>> Finish(long id)
        {
            try { return Ok(await _svc.FinishAsync(id)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
            catch (InvalidOperationException e) { return BadRequest(new { message = e.Message }); }
        }
    }
}
