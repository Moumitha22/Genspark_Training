using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twitter.Models
{
    public class TweetLike
    {
        [Key]
        public int SerialNumber { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("TweetId")]
        public int TweetId { get; set; }
        public Tweet? Tweet { get; set; }

        public DateTime LikedAt { get; set; }
    }
}

