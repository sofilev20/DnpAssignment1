using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;
    
    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository; 
    }

    public Task RunAsync(bool promptForUsernameFilter = false)
    {
        IQueryable<User> users = userRepository.GetMany();

        if (promptForUsernameFilter)
        {
            Console.Write("Search for username containing: ");
            string search = Console.ReadLine() ?? string.Empty;
            users = users.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        Console.WriteLine("--- Users ---");
        List<User> userList = users.ToList();

        if (!userList.Any())
        {
            Console.WriteLine("No users found.");
            return Task.CompletedTask;
        }

        foreach (User user in userList)
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }

        return Task.CompletedTask;
    }
}