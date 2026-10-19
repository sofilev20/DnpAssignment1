namespace ApiContracts.Comments;

public class UpdateCommentDto
{
    public required int Id { get; set; }
    public required string Body { get; set; }
}