using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data.Models;
using WebApi.Dtos;
using WebApi.Services.Auth;

namespace WebApi.Controllers;

public class AuthController(
    UserManager<User> userManager,
    IAuthService authService,
    RoleManager<IdentityRole> roleManager)
    : BaseController
{
    [AllowAnonymous]
    [HttpPost("sing-up", Name = "SingUp")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request, CancellationToken cancellationToken)
    {
        var existUser = await userManager.FindByEmailAsync(request.Email);
        if (existUser != null)
        {
            return BadRequest("User already exists");
        }

        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        var role = roles.FirstOrDefault(r => r.Name == request.Role);
        if (role == null)
        {
            return BadRequest("Role not exists");
        }
        var username = request.Email.Split('@')[0];

        var user = new User
        {
            FullName = request.FullName,
            UserName = username,
            Email = request.Email,
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var resultRole = await userManager.AddToRoleAsync(user, role.Name!);
        if (!resultRole.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return BadRequest(resultRole.Errors);
        }

        var rolesUser = await userManager.GetRolesAsync(user);

        var userResponse = new UserResponse
        {
            FullName = user.FullName,
            Username = user.UserName,
            Roles = rolesUser.ToList(),
            AccessToken = authService.CreateToken(user, rolesUser)
        };


        return Ok(userResponse);
    }

    [AllowAnonymous]
    [HttpPost("sign-in", Name = "SingIn")]
    public async Task<IActionResult> SignIn([FromBody] LoginDto request, CancellationToken cancellationToken)
    {
        var existUser = await userManager.FindByEmailAsync(request.Email);
        if (existUser == null)
        {
            return BadRequest("User not exists");
        }

        var result = await userManager.CheckPasswordAsync(existUser, request.Password);
        if (!result)
        {
            return BadRequest("Password incorrect");
        }

        var rolesUser = await userManager.GetRolesAsync(existUser);

        var userResponse = new UserResponse
        {
            FullName = existUser.FullName,
            Username = existUser.UserName!,
            Roles = rolesUser.ToList(),
            AccessToken = authService.CreateToken(existUser, rolesUser)
        };

        return Ok(userResponse);
    }
    [HttpGet("roles", Name = "Test")]
    [AllowAnonymous]
    public async Task<IActionResult> Test()
    {
        var roles = await roleManager.Roles.ToListAsync();
        return Ok(roles);
    }
}