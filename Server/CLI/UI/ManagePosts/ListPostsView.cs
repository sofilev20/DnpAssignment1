using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task RunAsync(int? filterByUserId = null)
    {
        IQueryable<Post> posts = postRepository.GetMany();

        if (filterByUserId.HasValue)
        {
            posts = posts.Where(p => p.UserId == filterByUserId.Value);
        }

        Console.WriteLine("Posts overview");

        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Title}, {post.Id}]");
        }

        return Task.CompletedTask;
    }
}