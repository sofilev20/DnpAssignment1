using ApiContracts.Comments;
using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    // POST /Posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        try
        {
            await VerifyUserExistsAsync(request.UserId);

            Post post = new(request.Title, request.Body, request.UserId);
            Post created = await postRepo.AddAsync(post);

            PostDto dto = ToDto(created);
            return Created($"/Posts/{dto.Id}", dto);
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

    // PUT /Posts/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            post.Title = request.Title;
            post.Body = request.Body;
            await postRepo.UpdateAsync(post);

            return Ok(ToDto(post));
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

    // GET /Posts/1?includeComments=true
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost([FromRoute] int id, [FromQuery] bool includeComments = false)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            PostDto dto = ToDto(post);

            if (includeComments)
            {
                dto.Comments = commentRepo.GetMany()
                    .Where(c => c.PostId == id)
                    .ToList()
                    .Select(ToCommentDto)
                    .ToList();
            }

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

    // GET /Posts?titleContains=cat&userId=1&userName=trmo
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetPosts(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        try
        {
            IQueryable<Post> posts = postRepo.GetMany();

            if (!string.IsNullOrWhiteSpace(titleContains))
            {
                posts = posts.Where(p => p.Title.ToLower().Contains(titleContains.ToLower()));
            }

            if (userId.HasValue)
            {
                posts = posts.Where(p => p.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<int> authorIds = userRepo.GetMany()
                    .Where(u => u.UserName.ToLower() == userName.ToLower())
                    .Select(u => u.Id)
                    .ToList();
                posts = posts.Where(p => authorIds.Contains(p.UserId));
            }

            List<PostDto> dtos = posts.ToList().Select(ToDto).ToList();
            return Ok(dtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // DELETE /Posts/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);
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

    // GET /Posts/1/comments  (comments belong to a post)
    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost([FromRoute] int postId)
    {
        try
        {
            await postRepo.GetSingleAsync(postId); // 404 if the post doesn't exist

            List<CommentDto> dtos = commentRepo.GetMany()
                .Where(c => c.PostId == postId)
                .ToList()
                .Select(ToCommentDto)
                .ToList();
            return Ok(dtos);
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

    private PostDto ToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorUserName = GetUserName(post.UserId)
        };
    }

    private CommentDto ToCommentDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            UserName = GetUserName(comment.UserId),
            PostId = comment.PostId
        };
    }

    private string GetUserName(int userId)
    {
        User? user = userRepo.GetMany().SingleOrDefault(u => u.Id == userId);
        return user?.UserName ?? "Unknown";
    }
}