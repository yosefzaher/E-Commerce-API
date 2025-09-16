using E_Commerce.BLL.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;
public interface IAuthService
{
    Task<AuthResponseDto?> GetTokenAsync(string email, string password, CancellationToken cancellation = default);
    Task<AuthResponseDto?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default);
    Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default);
    Task<AuthResponseDto?> RegisterAsync(RegirsterRequestDto request, CancellationToken cancellation = default);
}
