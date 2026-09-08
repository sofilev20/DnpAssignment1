using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class DeleteCommentView
{
    private readonly ICommentRepository commentRepository;
    
    public DeleteCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine(" Delete comment ");
        Console.Write("Enter the ID of the comment to delete: ");
        var idInput = Console.ReadLine();

        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await commentRepository.DeleteAsync(id);
            Console.WriteLine("Comment deleted.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}