using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WpfStudentChat.Server.Models;
using WpfStudentChat.Server.Models.Database;
using WpfStudentChat.Server.Services;

namespace WpfStudentChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public record LoginRequestData(string UserName, string PasswordHash);
        public record LoginResultData(string Token);


        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ApiResult<LoginResultData>> LoginAsync([FromBody] LoginRequestData request)
        {
            var token = await _authService.GetTokenAsync(request.UserName, request.PasswordHash);

            if (token is null)
            {
                return ApiResult<LoginResultData>.CreateErr();
            }
            else
            {
                return ApiResult<LoginResultData>.CreateOk(new LoginResultData(token));
            }
        }
    }
}
