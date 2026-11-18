namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int UserId { get; set; }  //foreign key
    public User User { get; set; } = null!; //navigation to the author
    
    //Navigation properties (EFC)
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    public Post(string title, string body, int userId)
    {
        Title = title;
        Body = body;
        UserId = userId; 
    }
    
    private Post(){}//for EFC
}