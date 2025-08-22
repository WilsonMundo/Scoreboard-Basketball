using AngularApp1.Server.Application.DTO.Game;
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
        public async Task<ActionResult<GameDto>> Create([FromBody] CreateGameDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GameDto>> GetById(long id)
        {
            var game = await _svc.GetAsync(id);
            return game is null ? NotFound() : Ok(game);
        }
    }
}
