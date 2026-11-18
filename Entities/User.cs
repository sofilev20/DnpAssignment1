namespace Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    
    //Navigation properties (EFC)
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    public User(string username, string password)
    {
        UserName = username;
        Password = password;
    }
    private  User()
    {
    }// for EFC
}