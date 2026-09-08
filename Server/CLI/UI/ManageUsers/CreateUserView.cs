using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;
    
    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.Write("Create new user");
        
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Password: ");
        string password = Console.ReadLine() ?? string.Empty;
        
        User user = new User(username, password);
        User created = await userRepository.AddAsync(user);
        
        Console.WriteLine($"User created with ID: {created.Id}");

    }
    
}