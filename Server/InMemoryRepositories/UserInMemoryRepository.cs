using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new List<User>();
    
    public UserInMemoryRepository()
    {
        _ = AddAsync(new User("trmo", "1234")).Result;
        _ = AddAsync(new User("mivi", "4321")).Result;
        _ = AddAsync(new User("jknr", "1243")).Result;
        _ = AddAsync(new User("alhe", "2143")).Result;
    }
    
    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any()
            ? users.Max(x => x.Id) + 1
            : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(x => x.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.Id}' not found");
        }
        
        users.Remove(existingUser);
        users.Add(user);
           
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    { 
        var userToRemove= users.SingleOrDefault(x => x.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }
        users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        var user = users.SingleOrDefault(x => x.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");  
        }
        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany()
    {
        return  users.AsQueryable();
    }
}