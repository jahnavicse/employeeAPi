using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Sujay_sir_Test_Emp_API.DTOs.EmployeeDtos;

namespace Sujay_sir_Test_Emp_API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var role = (request.Username, request.Password) switch
        {
            ("admin", "Admin123!") => "Admin",
            ("user", "User123!") => "User",
            _ => null
        };
        if (role is null) return Unauthorized();

        var key = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
            throw new InvalidOperationException("Set Jwt__Key to a secret of at least 32 bytes.");
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(configuration.GetValue<int>("Jwt:ExpiresMinutes", 60));
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, request.Username), new Claim(ClaimTypes.Name, request.Username), new Claim(ClaimTypes.Role, role) };
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, now, expires, new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expires, role));
    }
}

