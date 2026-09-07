using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new List<Comment>();

    public CommentInMemoryRepository()
    {
        _ = AddAsync(new Comment("Cats are great!", 1, 1)).Result;
        _ = AddAsync(new Comment("So true!", 1, 2)).Result;
        _ = AddAsync(new Comment("They're just so fluffy", 1, 2)).Result;
        _ = AddAsync(new Comment("Mine's hairless!", 1, 1)).Result;
        _ = AddAsync(new Comment("Is it sick?!", 1, 4)).Result;
        
        _ = AddAsync(new Comment("Cats are still great!", 2, 2)).Result;
        _ = AddAsync(new Comment("Man, mine just spat out a dead mouse :(", 2, 3)).Result;
        _ = AddAsync(new Comment("That's a compliment",2, 2)).Result;
        _ = AddAsync(new Comment("No rats around my house!", 2, 1)).Result;

        _ = AddAsync(new Comment("#FIRST", 3, 1)).Result;
        _ = AddAsync(new Comment("They're just so happy and loving", 3, 2)).Result;
        _ = AddAsync(new Comment("Too noisy for me!", 3, 4)).Result;
        _ = AddAsync(new Comment("Uhhh, no?? Cats forever", 3, 4)).Result;
        
        _ = AddAsync(new Comment("Weather is just the greatest!", 4, 4)).Result;
        _ = AddAsync(new Comment("Not today! It's raining :(", 4, 3)).Result;
        _ = AddAsync(new Comment("Rain just smells so nice", 4, 4)).Result;
        _ = AddAsync(new Comment("Weirdo :O", 4, 1)).Result;
        
        _ = AddAsync(new Comment("HELP!?", 5, 1)).Result;
        _ = AddAsync(new Comment("How do I even do anything?", 5, 1)).Result;
        _ = AddAsync(new Comment("I don't understand async", 5, 2)).Result;
        _ = AddAsync(new Comment("What do you need help with?", 5, 3)).Result;
        
        _ = AddAsync(new Comment("Eh, what? Your carpet?", 6, 2)).Result;
        _ = AddAsync(new Comment("I like my Mowinator3000, it just works", 6, 4)).Result;
        _ = AddAsync(new Comment("It just grows out of control!", 6, 3)).Result;
        _ = AddAsync(new Comment("What color is your carpet?", 6, 1)).Result;

    }
    
     public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(x => x.Id) + 1
            : 1;

        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(x => x.Id == comment.Id);

        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var commentToRemove = comments.SingleOrDefault(x => x.Id == id);

        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        var comment = comments.SingleOrDefault(x => x.Id == id);

        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}