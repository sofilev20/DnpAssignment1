using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ManageCommentsView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    
    public ManageCommentsView(ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }
    
    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Manage comments");
            Console.WriteLine("1. Add comment to post");
            Console.WriteLine("2. View all comments");
            Console.WriteLine("3. View comments by a specific user");
            Console.WriteLine("4. Update comment");
            Console.WriteLine("5. Delete comment");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    CreateCommentView createCommentView = new CreateCommentView(commentRepository, postRepository);
                    await createCommentView.RunAsync();
                    break;
                case "2":
                    ListCommentsView listCommentsView = new ListCommentsView(commentRepository);
                    await listCommentsView.RunAsync();
                    break;
                case "3":
                    Console.Write("User ID: ");
                    string? userIdInput = Console.ReadLine();
                    if (int.TryParse(userIdInput, out int userId))
                    {
                        ListCommentsView filteredCommentsView = new ListCommentsView(commentRepository);
                        await filteredCommentsView.RunAsync(userId);
                    }
                    else
                    {
                        Console.WriteLine("Invalid user ID.");
                    }
                    break;
                case "4":
                    UpdateCommentView updateCommentView = new UpdateCommentView(commentRepository);
                    await updateCommentView.RunAsync();
                    break;
                case "5":
                    DeleteCommentView deleteCommentView = new DeleteCommentView(commentRepository);
                    await deleteCommentView.RunAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown option, try again.");
                    break;
            }

            Console.WriteLine();
        }
    }
    
}