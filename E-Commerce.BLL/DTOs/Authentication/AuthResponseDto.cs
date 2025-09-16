using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.DTOs.Authentication;
public class AuthResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiration { get; set; }

    public bool RegisterIsSucceeded { get; set; } = true;
    public IEnumerable<string>? RegisterErrors { get; set; }

    public List<string> UserRoles { get; set; } = [];
}