using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    
    public CommentsController(ICommentRepository commentRepo)
    {
        this.commentRepo = commentRepo;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateComment([FromBody] UpdateCommentDto request)
    {
        Comment commentToUpdate = await commentRepo.GetSingleAsync(request.Id);
        commentToUpdate.Body = request.Body;
        await commentRepo.UpdateAsync(commentToUpdate); 
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteComment([FromRoute] int id)
    {
        await commentRepo.DeleteAsync(id); 
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment([FromRoute] int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);
        CommentDto dto = new()
        {
            Id = comment.Id,
            Body = comment.Body,
            AuthorUserId = comment.UserId,
            PostId = comment.PostId
        };
        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments(
        [FromQuery] int? userId = null,
        [FromQuery] int? postId = null)
    {  
        List<CommentDto> comments = commentRepo.GetMany()
            .Where(c => userId == null || c.UserId == userId)
            .Where(c => postId == null || c.PostId == postId)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                AuthorUserId = c.UserId,
                PostId = c.PostId
            })
            .ToList();

        return Ok(comments);
    }

}