using System.Text.Json;
using ApiContracts.Posts;

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

    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage httpResponse  = await client.PutAsJsonAsync($"posts/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }    
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"posts/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }    
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{id}?includeAuthor=true&includeComments=true");
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

    public async Task<List<PostDto>> GetPostsAsync()
    {
        HttpResponseMessage httpResponse = await client.GetAsync("posts");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<PostDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;    
    }
}