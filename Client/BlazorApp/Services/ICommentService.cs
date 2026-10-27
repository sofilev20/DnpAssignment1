using ApiContracts.Comments;
namespace BlazorApp.Services;


public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto request);
    Task UpdateCommentAsync(UpdateCommentDto request);
    Task DeleteCommentAsync(int id);
    Task<CommentDto> GetCommentAsync(int id);
    Task<List<CommentDto>> GetCommentsForPostAsync(int postId);
}