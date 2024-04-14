using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentChat.Server.Models;
using StudentChat.Server.Models.Database;

namespace StudentChat.Server.Services;

public class AuthService
{
    private readonly ChatServerDbContext _dbContext;
    private readonly IOptionsSnapshot<AppConfig> _appConfig;

    public AuthService(
        ChatServerDbContext dbContext,
        IOptionsSnapshot<AppConfig> appConfig)
    {
        _dbContext = dbContext;
        _appConfig = appConfig;
    }

    private string Sha256(string origin)
    {
        var bytes = Encoding.UTF8.GetBytes(origin);
        var summary = SHA256.HashData(bytes);

        return Convert.ToHexString(summary);
    }

    private bool IsManagerAccount(string userName, string passwordHash)
        => _appConfig.Value.ManagerUserName == userName && passwordHash.Equals(Sha256(_appConfig.Value.ManagerPassword), StringComparison.OrdinalIgnoreCase);

    private async Task<int> GetUserIdAsync(string userName, string passwordHash)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == userName && user.PasswordHash == passwordHash);
        if (user is null)
            return -1;

        return user.Id;
    }

    private string CreateToken(IEnumerable<Claim> claims)
    {
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Value.JwtSecret));
        var algorithm = SecurityAlgorithms.HmacSha256;
        var signingCredentials = new SigningCredentials(secret, algorithm);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _appConfig.Value.JwtIssuer,
            audience: _appConfig.Value.JwtAudience,
            claims: claims,
            DateTime.Now,
            DateTime.Now.AddDays(30),
            signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return tokenString;
    }

    private string CreateManagerToken()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim(ClaimTypes.Role, "Admin"),
        };

        return CreateToken(claims);
    }

    private string CreateNormalUserToken(string userName, int userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User"),
        };

        return CreateToken(claims);
    }

    public async Task<string?> GetTokenAsync(string userName, string passwordHash)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passwordHash))
            return null;

        if (IsManagerAccount(userName, passwordHash))
            return CreateManagerToken();

        int userId = await GetUserIdAsync(userName, passwordHash);
        if (userId < 0)
            return null;

        return CreateNormalUserToken(userName, userId);
    }
}
