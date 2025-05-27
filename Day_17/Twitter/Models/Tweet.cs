using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twitter.Models
{
    public class Tweet
{
    public int Id { get; set; }

    [Required]
    [MaxLength(280)]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }

    public ICollection<TweetLike>? Likes { get; set; }
    public ICollection<TweetHashtag>? TweetHashtags { get; set; }
}

    
}
