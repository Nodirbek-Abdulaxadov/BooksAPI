using AutoMapper;
using BooksAPI.DTOs.UserDtos;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogicLayer.Service;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager,
                       IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<AuthResult> RegisterUserAsync(RegisterUserDto dto, string role)
    {
        var user = _mapper.Map<User>(dto);
        user.EmailConfirmed = true;

        await _userManager.SetUserNameAsync(user, dto.Email);
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return (AuthResult)result;
        }

        result = await _userManager.AddToRoleAsync(user, role);
        return (AuthResult)result;
    }

    public async Task<LoginResult> LoginUserAsync(LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return new LoginResult(false, new List<string> 
                    { "Email doesn't exist in our database!" }
            );
        }

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (result)
        {
            var exsitingToken = await _userManager.GetAuthenticationTokenAsync(user, "BookStore", "Token");
            if (exsitingToken is not null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "BookStore", "Token");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user.FullName, user.UserName ?? "", roles.ToList());

            await _userManager.SetAuthenticationTokenAsync(user, "BookStore", "Token", token);

            return new LoginResult( true, new List<string>(), token);
        }

        return new LoginResult(false, new List<string> { "Password is incorrect!" } );
    }

    public string GenerateJwtToken(string fullName, string username, List<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("BuVapwemSecretKey"); // Same key as used in authentication configuration

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.GivenName, fullName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        foreach (var role in roles)
        {
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<AuthResult> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return new AuthResult(false, 
                   new List<string> { "Email doesn't exist in our database!" });
        }

        if (dto.CurrentPassword == dto.NewPassword)
        {
            return new AuthResult(false, 
                   new List<string> { "New password can't be the same as the current password!" });
        }

        bool currentPasswordIsValid = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
        if (!currentPasswordIsValid)
        {
            return new AuthResult(false, 
                   new List<string> { "Current password is incorrect!" });
        }

        var result = await _userManager.ChangePasswordAsync(user, 
                                                            dto.CurrentPassword, 
                                                            dto.NewPassword);

        return (AuthResult)result;
    }

    public async Task<AuthResult> DeleteAccountAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new AuthResult(false, 
                   new List<string> { "Email doesn't exist in our database!" });
        }

        var exsitingToken = await _userManager.GetAuthenticationTokenAsync(user, "BookStore", "Token");
        if (exsitingToken is not null)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "BookStore", "Token");
        }

        var result = await _userManager.DeleteAsync(user);
        return (AuthResult)result;
    }
}