using System.Text.Json;
using ApiContracts.Posts;
using ApiContracts.Users;
using ApiContracts.Comments;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;
    
    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }
    
    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<IEnumerable<PostDto>> GetPosts()
    {
        HttpResponseMessage httpResponse = await client.GetAsync("posts");
        string response = await httpResponse.Content.ReadAsStringAsync();
       
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        
        return JsonSerializer.Deserialize<IEnumerable<PostDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    public async Task<PostDto> GetSinglePost(int postId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}");
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    // This method might belong to the IUserService instead.
    // But, I'm getting the author of a post, so I put it here.
    public async Task<UserDto> GetAuthorOfPost(int postId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}/author");
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    // And this method might belong to a ICommentService instead.
    // Same reasoning as above. I'm getting comments for a _post_, so I put it here.
    // If I were to get comments for a user, I would put it in the IUserService.
    // If I were interacting with comment without user or post, I would probably use a comment service.
    // It's your decision, there is not necessarily a right or wrong here. But consistency is at least important. 
    public async Task<List<CommentDto>> GetCommentsForPost(int postId)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}/comments");
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<CommentDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<CommentDto> AddComment(CreateCommentDto createCommentDto)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync($"posts/{createCommentDto.PostId}/comments", createCommentDto);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<CommentDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    
    

   
}