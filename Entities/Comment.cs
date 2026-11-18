namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = null!;
    public int UserId { get; set; } //foreign key -> author
    public User User { get; set; } = null!; //navigation to the author
    public int PostId { get; set; } // foreign key -> post
    public Post Post { get; set; } = null!; //navigation to the post

    public Comment(string body, int userId, int postId)
    {
        Body = body;
        UserId = userId;
        PostId = postId;
    }
    
    private Comment(){}
}