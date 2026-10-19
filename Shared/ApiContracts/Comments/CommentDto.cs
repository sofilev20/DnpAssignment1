namespace ApiContracts.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int AuthorUserId { get; set; }
    public int PostId { get; set; }
}