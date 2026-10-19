namespace ApiContracts.Comments;

public class CreateCommentDto
{
    public required string Body { get; set; }
    public required int AuthorUserId { get; set; }
    public required int PostId { get; set; }   
}