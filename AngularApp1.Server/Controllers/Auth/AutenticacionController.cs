using AngularApp1.Server.Application.DTO.Auth;
using AngularApp1.Server.Application.Interfaces;
using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Identity;


namespace AngularApp1.Server.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutenticacionController:ControllerBase
    {
        private readonly ResponseService _response;
        private readonly IAuthService _IAuthService;
        public AutenticacionController(ResponseService response, IAuthService iAuthService)
        {
            this._response = response;
            this._IAuthService = iAuthService;
        }

        [HttpGet("auth/verificar")]
        [AllowAnonymous]
        public ActionResult getAutenticacion()
        {
            if (User.Identity != null)
                if (User.Identity.IsAuthenticated)
                {
                    var userInfo = new UserInfoModel
                    {
                        Name = User.Identity.Name,
                        Claims = User.Claims.Select(c => new UserInfoModel.ClaimModel { Type = c.Type, Value = c.Value })

                    };

                    return Ok(userInfo);
                }
            return Ok(new UserInfoModel { });
        }
        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLogin([FromBody] UserInfo userInfo)
        {
            try
            {
                ResultAPI<UserToken?> userToken = await _IAuthService.ValidateUser(userInfo);
                if (userToken.Result != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = userToken.Result.Expiration,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };

                    Response.Cookies.Append("AuthToken", userToken.Result.Token, cookieOptions);
                    return Ok(new
                    {
                        token = userToken.Result.Token,
                        expiration = userToken.Result.Expiration
                    });

                }
                else
                {
                    return _response.CreateResponse(userToken);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al autenticar login");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno comunicar a soporte");
            }
        }

        [HttpPost("Auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginRest([FromBody] UserInfo userInfo)
        {
            try
            {
                ResultAPI<UserToken?> userToken = await _IAuthService.ValidateUser(userInfo);
                if (userToken.Result != null)
                {
                    return _response.CreateResponse(userToken);
                }
                else
                {
                    return _response.CreateResponse(userToken);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al autenticar login");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno comunicar a soporte");
            }
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister user)
        {
            ResultAPI<object> result = await _IAuthService.RegisterUser(user);
            return _response.CreateResponse(result);
        }

    }
}
