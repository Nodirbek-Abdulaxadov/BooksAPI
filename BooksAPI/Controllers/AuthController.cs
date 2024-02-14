using BooksAPI.DTOs.UserDtos;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await _userService.RegisterUserAsync(dto, "User");
        if (result.IsSuccessed)
        {
            return Ok("User Created!");
        }

        return BadRequest(result.ErrorMessages);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAdmin(RegisterUserDto dto)
    {
        var result = await _userService.RegisterUserAsync(dto, "Admin");
        if (result.IsSuccessed)
        {
            return Ok("User Created!");
        }

        return BadRequest(result.ErrorMessages);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await _userService.LoginUserAsync(dto);
        if (result.IsSuccessed)
        {
            return Ok(result.Token);
        }

        return BadRequest(result.ErrorMessages);
    }

    [HttpPut("[action]")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var result = await _userService.ChangePasswordAsync(dto);
        if (result.IsSuccessed)
        {
            return Ok("Password Changed!");
        }

        return BadRequest(result.ErrorMessages);
    }

    [HttpDelete("[action]")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount(string email)
    {
        var result = await _userService.DeleteAccountAsync(email);
        if (result.IsSuccessed)
        {
            return Ok("Account Deleted!");
        }

        return BadRequest(result.ErrorMessages);
    }
}
