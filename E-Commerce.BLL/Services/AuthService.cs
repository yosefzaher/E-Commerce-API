using AutoMapper;
using E_Commerce.BLL.Abstraction.Roles;
using E_Commerce.BLL.Authentication;
using E_Commerce.BLL.DTOs.Authentication;
using E_Commerce.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;
public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, IMapper mapper) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IMapper _mapper = mapper;

    private readonly int _refreshTokenExpireDays = 14;

    public async Task<AuthResponseDto?> GetTokenAsync(string email, string password,
        CancellationToken cancellation = default)
    {
        // Check user by using email?
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        // Check the password
        var isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid)
            return null;

        // Generate JWT Token
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        // Call Refresh Token Genrator Function
        var refreshToken = GenerateRefreshToke();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpireDays);

        // Add Refresh Token to DB
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        // Get user roles
        var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

        // Return Token & Refresh Token
        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            ExpiresIn = expiresIn,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshTokenExpiration,
            UserRoles = userRoles 
        };

    }

    public async Task<AuthResponseDto?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return null;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return null;

        // Revoke the old refresh token
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        // Generate New JWT Token
        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

        // Call Refresh Token Genrator Function to Generate New refresh token
        var newRefreshToken = GenerateRefreshToke();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpireDays);

        // Add Refresh Token to DB
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        // Get user roles
        var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

        // Return Token & Refresh Token
        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = newToken,
            ExpiresIn = expiresIn,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiration = refreshTokenExpiration,
            UserRoles = userRoles
        };
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegirsterRequestDto request, CancellationToken cancellation = default)
    {
        var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellation);

        if (emailIsExist)
        {
            return new AuthResponseDto
            {
                RegisterIsSucceeded = false,
                RegisterErrors = new[] {"Email is already Exist"}
            };
        }

        var user = _mapper.Map<ApplicationUser>(request);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                RegisterIsSucceeded = false,
                RegisterErrors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _userManager.AddToRoleAsync(user, Roles.Customer);

        // Generate Token & Refresh
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToke();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpireDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            ExpiresIn = expiresIn,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshTokenExpiration,
            UserRoles = userRoles,
            RegisterIsSucceeded = true
        };
    }

    // Generate Refresh Token
    private string GenerateRefreshToke()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
