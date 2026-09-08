using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    
    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task RunAsync()
    {
        Console.WriteLine("Create new post");

        Console.Write("Title: ");
        string title = Console.ReadLine() ?? string.Empty;

        Console.Write("Body: ");
        string body = Console.ReadLine() ?? string.Empty;

        Console.Write("User ID: ");
        var userIdInput = Console.ReadLine();
        int.TryParse(userIdInput, out int userId);

        Post post = new Post(title, body, userId);
        Post created = await postRepository.AddAsync(post);

        Console.WriteLine($"Post created with ID {created.Id}.");
    }
}