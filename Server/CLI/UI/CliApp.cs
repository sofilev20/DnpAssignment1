using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CliApp(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("=== DNP Forum CLI ===");
            Console.WriteLine("1. Manage users");
            Console.WriteLine("2. Manage posts");
            Console.WriteLine("3. Manage comments");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ManageUsersView manageUsersView = new ManageUsersView(userRepository);
                    await manageUsersView.RunAsync();
                    break;
                case "2":
                    ManagePostsView managePostsView = new ManagePostsView(postRepository, commentRepository);
                    await managePostsView.RunAsync();
                    break;
                case "3":
                    ManageCommentsView manageCommentsView = new ManageCommentsView(commentRepository, postRepository);
                    await manageCommentsView.RunAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown option, try again.");
                    break;
            }
        }

        Console.WriteLine("Goodbye!");
    }
}