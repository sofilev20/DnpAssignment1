using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UpdateUserView
{
    private readonly IUserRepository userRepository;
    
    public UpdateUserView(IUserRepository userRepository)
    { 
        this.userRepository = userRepository;
    }
    
    public async Task RunAsync()
    {
        Console.WriteLine("Update user");
        Console.Write("Enter the ID of the user to update: ");
        string? idInput = Console.ReadLine();

        if (!int.TryParse(idInput, out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        User existing;
        try
        {
            existing = await userRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.Write("New username: ");
        existing.UserName = Console.ReadLine() ?? string.Empty;

        Console.Write("New password: ");
        existing.Password = Console.ReadLine() ?? string.Empty;

        await userRepository.UpdateAsync(existing);
        Console.WriteLine("User updated.");
    }
}