using AngularApp1.Server.Application.DTO.Players;
using AngularApp1.Server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _svc;
        private readonly ResponseService _response;
        public PlayersController(IPlayerService svc, ResponseService response) { _svc = svc; _response = response; }


        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PlayerListRequest req)
        => _response.CreateResponse(await _svc.ListAsync(req));


        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        => _response.CreateResponse(await _svc.GetByIdAsync(id));


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlayerCreateDto dto)
        => _response.CreateResponse(await _svc.CreateAsync(dto));


        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] PlayerUpdateDto dto)
        => _response.CreateResponse(await _svc.UpdateAsync(id, dto));


        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        => _response.CreateResponse(await _svc.DeleteAsync(id));
    }
}
