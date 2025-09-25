using AngularApp1.Server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AngularApp1.Server.Application.DTO.Matches.Match;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class MatchesController : ControllerBase
    {
        private readonly IAdminMatchService _svc;
        private readonly ResponseService _response;
        public MatchesController(IAdminMatchService svc, ResponseService response) { _svc = svc; _response = response; }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        => _response.CreateResponse(await _svc.CreateFromCatalogAsync(dto));


        [HttpPost("{gameId:long}/roster")]
        public async Task<IActionResult> AssignRoster(long gameId, [FromBody] AssignRosterDto dto)
        => _response.CreateResponse(await _svc.AssignRosterAsync(gameId, dto));


        [HttpGet("history")]
        public async Task<IActionResult> History()
        => _response.CreateResponse(await _svc.GetHistoryAsync());
    }
}
