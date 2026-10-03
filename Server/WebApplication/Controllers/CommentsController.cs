using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;
    
    
    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    // POST /Comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        try
        {
            await VerifyUserExistsAsync(request.UserId);
            await VerifyPostExistsAsync(request.PostId);

            Comment comment = new(request.Body, request.UserId, request.PostId);
            Comment created = await commentRepo.AddAsync(comment);

            CommentDto dto = ToDto(created);
            return Created($"/Comments/{dto.Id}", dto);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // PUT /Comments/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            comment.Body = request.Body;
            await commentRepo.UpdateAsync(comment);

            return Ok(ToDto(comment));
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

    // GET /Comments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment([FromRoute] int id)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            return Ok(ToDto(comment));
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

    // GET /Comments?userId=1&userName=trmo&postId=2
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        try
        {
            IQueryable<Comment> comments = commentRepo.GetMany();

            if (userId.HasValue)
            {
                comments = comments.Where(c => c.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<int> authorIds = userRepo.GetMany()
                    .Where(u => u.UserName.ToLower() == userName.ToLower())
                    .Select(u => u.Id)
                    .ToList();
                comments = comments.Where(c => authorIds.Contains(c.UserId));
            }

            if (postId.HasValue)
            {
                comments = comments.Where(c => c.PostId == postId.Value);
            }

            List<CommentDto> dtos = comments.ToList().Select(ToDto).ToList();
            return Ok(dtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // DELETE /Comments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);
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

    private async Task VerifyUserExistsAsync(int userId)
    {
        bool exists = userRepo.GetMany().Any(u => u.Id == userId);
        if (!exists)
        {
            throw new ArgumentException($"User with ID '{userId}' does not exist");
        }

        await Task.CompletedTask;
    }

    private async Task VerifyPostExistsAsync(int postId)
    {
        bool exists = postRepo.GetMany().Any(p => p.Id == postId);
        if (!exists)
        {
            throw new ArgumentException($"Post with ID '{postId}' does not exist");
        }

        await Task.CompletedTask;
    }

    private CommentDto ToDto(Comment comment)
    {
        User? user = userRepo.GetMany().SingleOrDefault(u => u.Id == comment.UserId);
        return new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            UserName = user?.UserName ?? "Unknown",
            PostId = comment.PostId
        };
    }

}