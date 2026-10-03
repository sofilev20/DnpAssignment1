using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private const string FilePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync(); // Calling helper method
        comment.Id= comments.Count > 0 ? comments.Max(c => c.Id) : 1;
        comments.Add(comment);
        await SaveCommentsAsync(comments); // a helper method
        return comment;
    }

    private static Task SaveCommentsAsync(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true }); // The options make the JSON better formatted in the file, for easier readability.
        return File.WriteAllTextAsync(FilePath, commentsAsJson); // instead of awaiting the task, I can just return it. The caller of this method can await it instead. E.g. see the above method.
    }

    private static async Task<List<Comment>> LoadCommentsAsync()
    {
        string commentsAsJson = await File.ReadAllTextAsync(FilePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments;
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);

        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with id {id} not found");
        }
        return comment;
    }
    
    
    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();
        Comment existingComment = await GetSingleAsync(comment.Id);
       
        comments.Remove(existingComment);
        comments.Add(comment);
       
        await SaveCommentsAsync(comments);
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException($"Comment with id {id} not found"); 
        }
        
        comments.Remove(commentToRemove);
        await SaveCommentsAsync(comments);
    }
    
    public IQueryable<Comment> GetMany()
        => LoadCommentsAsync().Result.AsQueryable();
  
}