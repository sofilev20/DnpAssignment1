using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class UpdateCommentView
{
    private readonly ICommentRepository commentRepository;

    public UpdateCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine(" Update comment ");
        Console.Write("Enter the ID of the comment to update: ");
        var idInput = Console.ReadLine();

        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Comment existing;
        try
        {
            existing = await commentRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.Write("New body: ");
        existing.Body = Console.ReadLine() ?? string.Empty;

        await commentRepository.UpdateAsync(existing);
        Console.WriteLine("Comment updated.");
    }
}