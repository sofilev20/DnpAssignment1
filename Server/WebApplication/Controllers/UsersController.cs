using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication.Controllers;

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
        try
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
     // PUT /Users/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);

            if (!string.Equals(user.UserName, request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                await VerifyUserNameIsAvailableAsync(request.UserName);
            }

            user.UserName = request.UserName;
            user.Password = request.Password;
            await userRepo.UpdateAsync(user);

            UserDto dto = new()
            {
                Id = user.Id,
                UserName = user.UserName
            };
            return Ok(dto);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /Users/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);
            UserDto dto = new()
            {
                Id = user.Id,
                UserName = user.UserName
            };
            return Ok(dto);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /Users?userNameContains=abc
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? userNameContains)
    {
        try
        {
            IQueryable<User> users = userRepo.GetMany();

            if (!string.IsNullOrWhiteSpace(userNameContains))
            {
                users = users.Where(u => u.UserName.ToLower().Contains(userNameContains.ToLower()));
            }

            List<UserDto> dtos = users
                .Select(u => new UserDto { Id = u.Id, UserName = u.UserName })
                .ToList();
            return Ok(dtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // DELETE /Users/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
        bool taken = userRepo.GetMany()
            .Any(u => u.UserName.ToLower() == userName.ToLower());

        if (taken)
        {
            throw new ArgumentException($"Username '{userName}' is already taken");
        }

        await Task.CompletedTask;
    }
}