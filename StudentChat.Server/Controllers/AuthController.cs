using StudentChat.Models.Network;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentChat.Server.Services;

namespace StudentChat.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;


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
            return ApiResult<LoginResultData>.CreateErr("Invalid user name or password hash");
        }
        else
        {
            return ApiResult<LoginResultData>.CreateOk(new LoginResultData(token));
        }
    }
}
