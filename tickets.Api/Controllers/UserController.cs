using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tickets.Api.Dtos;
using tickets.Api.Services;

namespace tickets.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [AllowAnonymous]
public class UserController(UserService usrService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRequest request) =>
        await usrService.Register(request)
            ? Ok(new { message = "User registered successfully." })
            : Conflict("Username already exists.");

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserRequest request)
    {
        var response = await usrService.Login(request);
        return response is not null ? Ok(response) : Unauthorized("Invalid username or password.");
    }

    // [Authorize]
    [HttpGet("me")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCurrentUser() => Ok(new { User.Identity?.Name });
    // $"IsAdmin = {User.IsInRole("Admin")}, Name = {User.Identity?.Name"}
    // [Authorize(Roles = "Admin")]
    // [HttpGet("me")]
    // public IActionResult GetCurrentUser()
    // {
    //     foreach (var c in User.Claims)
    //         Console.WriteLine($"{c.Type} = {c.Value}");
    //
    //     return Ok(new { username = User.Claims.FirstOrDefault(c => c.Type == "Name")?.Value });
    // }
}
