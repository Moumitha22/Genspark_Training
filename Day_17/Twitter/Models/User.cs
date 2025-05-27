namespace Twitter.Models
{
 public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string Status { get; set; } = string.Empty;

    public ICollection<Tweet>? Tweets { get; set; }
    public ICollection<TweetLike>? Likes { get; set; }
    public ICollection<UserFollow>? Followers { get; set; } 
    public ICollection<UserFollow>? Following { get; set; } 
}
   
}


