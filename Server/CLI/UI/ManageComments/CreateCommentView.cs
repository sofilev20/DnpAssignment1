using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    
    public CreateCommentView(ICommentRepository commentRepository, IPostRepository postRepository)
        {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        }
   
    public async Task RunAsync()
    {
        Console.WriteLine(" Add comment to existing post ");

        Console.Write("Post ID: ");
        var postIdInput = Console.ReadLine();

        if (!int.TryParse(postIdInput, out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        try
        {
            await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.Write("Body: ");
        string body = Console.ReadLine() ?? string.Empty;

        Console.Write("User ID: ");
        var userIdInput = Console.ReadLine();
        int.TryParse(userIdInput, out int userId);

        Comment comment = new Comment(body, postId, userId);
        Comment created = await commentRepository.AddAsync(comment);

        Console.WriteLine($"Comment created with ID {created.Id}.");
    }
    
}