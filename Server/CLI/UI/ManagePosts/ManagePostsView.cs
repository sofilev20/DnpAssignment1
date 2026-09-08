using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public ManagePostsView(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Manage posts ");
            Console.WriteLine("1. Create new post");
            Console.WriteLine("2. View posts overview");
            Console.WriteLine("3. View posts by a specific user");
            Console.WriteLine("4. View single post");
            Console.WriteLine("5. Update post");
            Console.WriteLine("6. Delete post");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    CreatePostView createPostView = new CreatePostView(postRepository);
                    await createPostView.RunAsync();
                    break;
                case "2":
                    ListPostsView listPostsView = new ListPostsView(postRepository);
                    await listPostsView.RunAsync();
                    break;
                case "3":
                    Console.Write("User ID: ");
                    var userIdInput = Console.ReadLine();
                    if (int.TryParse(userIdInput, out int userId))
                    {
                        ListPostsView filteredPostsView = new ListPostsView(postRepository);
                        await filteredPostsView.RunAsync(userId);
                    }
                    else
                    {
                        Console.WriteLine("Invalid user ID.");
                    }
                    break;
                case "4":
                    ViewPostView viewPostView = new ViewPostView(postRepository, commentRepository);
                    await viewPostView.RunAsync();
                    break;
                case "5":
                    UpdatePostView updatePostView = new UpdatePostView(postRepository);
                    await updatePostView.RunAsync();
                    break;
                case "6":
                    DeletePostView deletePostView = new DeletePostView(postRepository);
                    await deletePostView.RunAsync();
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