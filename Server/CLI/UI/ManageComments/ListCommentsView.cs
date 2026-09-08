using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentsView
{
    private readonly ICommentRepository commentRepository;

    public ListCommentsView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public Task RunAsync(int? filterByUserId = null)
    {
        IQueryable<Comment> comments = commentRepository.GetMany();

        if (filterByUserId.HasValue)
        {
            comments = comments.Where(c => c.UserId == filterByUserId.Value);
        }

        Console.WriteLine("--- Comments ---");
        List<Comment> commentList = comments.ToList();

        if (!commentList.Any())
        {
            Console.WriteLine("No comments found.");
            return Task.CompletedTask;
        }

        foreach (Comment comment in commentList)
        {
            Console.WriteLine($"[{comment.Id}] (Post {comment.PostId}, User {comment.UserId}): {comment.Body}");
        }

        return Task.CompletedTask;
    }
}