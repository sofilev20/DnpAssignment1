using ApiContracts.Comments;
using ApiContracts.Posts;
using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;

    public PostsController(IPostRepository postRepo)
    {
        this.postRepo = postRepo;
    }
    
    [HttpPost]
    public async Task<IResult> AddPost(
        [FromBody] CreatePostDto request,
        [FromServices] IUserRepository userRepo)
    {
        await VerifyAuthorExists(request.AuthorUserId, userRepo);

        Post post = new(request.Title, request.Body, request.AuthorUserId);
        Post created = await postRepo.AddAsync(post);

        PostDto dto = new()
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };
        return Results.Created($"/Posts/{created.Id}", dto);
    }
    
    private async Task VerifyAuthorExists(int userId, IUserRepository userRepo)
    {
        _ = await userRepo.GetSingleAsync(userId);
    }

    [HttpPut("{id:int}")]
    public async Task<IResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        Post postToUpdate = await postRepo.GetSingleAsync(id);

        postToUpdate.Title = request.Title;
        postToUpdate.Body = request.Body;
        await postRepo.UpdateAsync(postToUpdate);
        return Results.NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePost([FromRoute] int id)
    {
        await postRepo.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IResult> GetPost(
        [FromRoute] int id,
        [FromQuery] bool includeAuthor,
        [FromQuery] bool includeComments)
    {
        IQueryable<Post> queryForPost = postRepo
            .GetMany()
            .Where(p => p.Id == id)
            .AsQueryable();

        if (includeAuthor)
        {
            queryForPost = queryForPost.Include(p => p.User);
        }

        if (includeComments)
        {
            queryForPost = queryForPost.Include(p => p.Comments);
        }

        PostDto? dto = await queryForPost.Select(post => new PostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                Author = includeAuthor
                    ? new UserDto
                    {
                        Id = post.User.Id,
                        UserName = post.User.UserName
                    }
                    : null,
                Comments = includeComments
                    ? post.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Body = c.Body,
                        AuthorUserId = c.UserId
                    }).ToList()
                    : new ()
            })
            .FirstOrDefaultAsync();

        return dto == null ? Results.NotFound() : Results.Ok(dto);
    }

    [HttpGet]
    public async Task<IResult> GetPosts([FromQuery] string? titleContains = null, [FromQuery] int? userId = null)
    {
        IQueryable<Post> queryablePosts = postRepo.GetMany();
        if (titleContains != null)
        {
            queryablePosts = queryablePosts.Where(p => p.Title.Contains(titleContains));
        }

        if (userId != null)
        {
            queryablePosts = queryablePosts.Where(p => p.UserId == userId);
        }

        // using Select() here. It's a simpler way to convert a list of objects to a list of other objects.
        List<PostDto> posts = await queryablePosts.Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            })
            .ToListAsync();
        return Results.Ok(posts);
    }

    [HttpPost("{postId:int}/comments")] // This will result in this route: /Posts/{postId}/Comments
    public async Task<ActionResult<CommentDto>> AddComment(
        [FromRoute] int postId,
        [FromBody] CreateCommentDto request,
        [FromServices] IUserRepository userRepo,
        [FromServices] ICommentRepository commentRepo)
    {
        await VerifyPostExists(postId);
        await VerifyAuthorExists(request.AuthorUserId, userRepo);

        // could validate incoming data here, or in a business logic layer

        Comment comment = new(request.Body, request.AuthorUserId, postId);
        Comment created = await commentRepo.AddAsync(comment);
        CommentDto dto = new()
        {
            Id = created.Id,
            Body = created.Body,
            AuthorUserId = created.UserId,
            PostId = created.PostId
        };
        return Created($"/Comments/{created.Id}", dto);
    }

    private async Task VerifyPostExists(int postId)
    {
        _ = await postRepo.GetSingleAsync(postId);
    }
    
// This endpoint returns the comments of a specific post
    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(
        [FromRoute] int postId,
        [FromServices] ICommentRepository commentRepo)
    {
        await VerifyPostExists(postId);
        List<CommentDto> comments = await commentRepo.GetMany()
            .Where(c => c.PostId == postId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                AuthorUserId = c.UserId,
                PostId = c.PostId
            })
            .ToListAsync();
        return Ok(comments);
    }

    [HttpGet("{postId:int}/author")]
    public async Task<ActionResult<UserDto>> GetAuthorOfPost(
        [FromRoute] int postId,
        [FromServices] IUserRepository userRepo)
    {
        Post post = await postRepo.GetSingleAsync(postId);
        User author = await userRepo.GetSingleAsync(post.UserId);
        UserDto dto = new()
        {
            Id = author.Id,
            UserName = author.UserName,
        };
        return Ok(dto);
    }
}
