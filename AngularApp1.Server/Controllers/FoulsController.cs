using AngularApp1.Server.Application.DTO.Game;
using AngularApp1.Server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class FoulsController : ControllerBase
    {
        private readonly IGameService _svc;
        public FoulsController(IGameService svc) => _svc = svc;

        [HttpPost("{id:long}/teams/{teamId:long}/fouls")]
        public async Task<ActionResult<GameDto>> UpdateFouls(long id, long teamId, [FromBody] int delta)
        {
            try { return Ok(await _svc.UpdateFoulsAsync(id, teamId, delta)); }
            catch (KeyNotFoundException e) { return NotFound(new { message = e.Message }); }
        }
    }
}
