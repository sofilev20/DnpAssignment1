using System.ComponentModel.DataAnnotations;
using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;
    
    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    //POST /Users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    { 
        await VerifyUserNameIsAvailableAsync(request.UserName);

        User user = new(request.UserName, request.Password);
        User created = await userRepo.AddAsync(user);
        UserDto dto = new()
        { 
            Id = created.Id, 
            UserName = created.UserName
        }; 
        return Created($"/Users/{dto.Id}", created);
    }
    
    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
        bool usernameIsTaken = userRepo.GetMany()
            .Any(u => u.UserName.ToLower().Equals(userName.ToLower()));
        if (usernameIsTaken)
        {
            throw new ValidationException($"Username '{userName}' is already taken");
        }
    }
    
     // PUT /Users/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        User existing = await userRepo.GetSingleAsync(id);
        if (!string.Equals(existing.UserName, request.UserName, StringComparison.OrdinalIgnoreCase)) 
        {
            await VerifyUserNameIsAvailableAsync(request.UserName);
        }

        existing.UserName = request.UserName;
        existing.Password = request.Password;
        await userRepo.UpdateAsync(existing);
        return NoContent();
    }

    // DELETE /Users/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        await userRepo.DeleteAsync(id);
        return NoContent();
    }
    
    // GET /Users/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        User user = await userRepo.GetSingleAsync(id);
        return Ok(user);
    }

    // GET /Users?userNameContains=abc
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? userNameContains)
    {
        IQueryable<User> users = userRepo.GetMany()
            .Where(
                u => userNameContains == null ||
                     u.UserName.ToLower().Contains(userNameContains.ToLower())
            );

        return Ok(users);
    }
}