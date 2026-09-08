using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class UpdatePostView
{
    public readonly IPostRepository postRepository;
    
    public UpdatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Update post");
        Console.Write("Enter the ID of the post you want to update: ");
        var idInput = Console.ReadLine();
        
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }
        
        Post existing;
        try
        {
            existing = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
        
        Console.Write("New title: ");
        existing.Title = Console.ReadLine() ?? string.Empty;

        Console.Write("New body: ");
        existing.Body = Console.ReadLine() ?? string.Empty;

        await postRepository.UpdateAsync(existing);
        Console.WriteLine("Post updated.");
        
    }
}