using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
     private readonly IUserRepository userRepository;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Manage users");
            Console.WriteLine("1. Create new user");
            Console.WriteLine("2. View all users");
            Console.WriteLine("3. Search users by username");
            Console.WriteLine("4. Update user");
            Console.WriteLine("5. Delete user");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    CreateUserView createUserView = new CreateUserView(userRepository);
                    await createUserView.RunAsync();
                    break;
                case "2":
                    ListUsersView listUsersView = new ListUsersView(userRepository);
                    await listUsersView.RunAsync();
                    break;
                case "3":
                    ListUsersView searchUsersView = new ListUsersView(userRepository);
                    await searchUsersView.RunAsync(promptForUsernameFilter: true);
                    break;
                case "4":
                    UpdateUserView updateUserView = new UpdateUserView(userRepository);
                    await updateUserView.RunAsync();
                    break;
                case "5":
                    DeleteUserView deleteUserView = new DeleteUserView(userRepository);
                    await deleteUserView.RunAsync();
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