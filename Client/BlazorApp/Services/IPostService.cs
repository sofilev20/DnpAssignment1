using ApiContracts.Posts;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> AddPostAsync(CreatePostDto request);
    public Task UpdatePostAsync(int id, UpdatePostDto request);
    public Task DeletePostAsync(int id);
    public Task<PostDto> GetPostAsync(int id);
    public Task<List<PostDto>> GetPostsAsync();
}