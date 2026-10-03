using ApiContracts.Comments;

namespace ApiContracts.Posts;

public class PostDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required int UserId { get; set; }
    public required string AuthorUserName { get; set; }
    public List<CommentDto> Comments { get; set; }
}