using E_Commerce.BLL.DTOs.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (authResult is not null && !string.IsNullOrEmpty(authResult.RefreshToken))
            SetRefreshTokenCookies(authResult.RefreshToken, authResult.RefreshTokenExpiration);


        return authResult is null ? BadRequest("Invalid Email or Password.") : Ok(authResult);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (authResult is not null && !string.IsNullOrEmpty(authResult.RefreshToken))
            SetRefreshTokenCookies(authResult.RefreshToken, authResult.RefreshTokenExpiration);

        return authResult is null ? BadRequest("Token is revoked") : Ok(authResult);

    }

    [HttpPut("Revoke-RefreshToken")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked ? Ok() : BadRequest("Operation failed");
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RefreshToken([FromBody] RegirsterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (!result!.RegisterIsSucceeded)
            BadRequest(result.RegisterErrors);

        return Ok(result);
    }

    private void SetRefreshTokenCookies(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime()
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

}
