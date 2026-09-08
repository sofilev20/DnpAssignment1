using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class DeleteUserView
{
    private readonly IUserRepository userRepository;
    
    public DeleteUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Delete user");
        Console.Write("Enter the ID of the user you want to delete: ");
        string? userId = Console.ReadLine();

        if (!int.TryParse(userId, out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine($"User {id} deleted");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message); 
        }
    }
}