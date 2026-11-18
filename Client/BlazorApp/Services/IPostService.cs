using ApiContracts.Comments;
using ApiContracts.Posts;
using ApiContracts.Users;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<PostDto> AddPostAsync(CreatePostDto request);
    Task<IEnumerable<PostDto>> GetPosts();
    Task<PostDto> GetSinglePost(int postId);
    Task<UserDto> GetAuthorOfPost(int authorId);
    Task<List<CommentDto>> GetCommentsForPost(int postId);
    Task<CommentDto> AddComment(CreateCommentDto createCommentDto);
}