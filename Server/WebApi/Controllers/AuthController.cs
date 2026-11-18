using ApiContracts.Auth;
using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;


namespace WebApi.Controllers;

[ApiController ]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;
    
    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        User? user = await userRepo.GetMany()
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == request.UserName.ToLower());

        if (user == null)
        {
            return Unauthorized("User not found");
        }

        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password");
        }

        UserDto dto = new()
        {
            Id = user.Id,
            UserName = user.UserName
        };
        return Ok(dto);
    }
}