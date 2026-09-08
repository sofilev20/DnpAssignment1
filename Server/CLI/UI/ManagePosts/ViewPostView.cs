using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ViewPostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    
    public ViewPostView(IPostRepository postRepository, ICommentRepository commentRepository)
        {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        }

    public async Task RunAsync()
    {
        Console.Write("Enter the ID of the post to view: ");
        var idInput = Console.ReadLine();
        
        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");

        Console.WriteLine("Comments:");
        IEnumerable<Comment> comments = commentRepository.GetMany().Where(c => c.PostId == post.Id);

        foreach (Comment comment in comments)
        {
            Console.WriteLine($"- {comment.Body}");
        }
    }
}