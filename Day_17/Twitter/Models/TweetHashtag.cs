using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twitter.Models
{

    public class TweetHashtag
    {
        [Key]
        public int SerialNumber { get; set; }
        public int TweetId { get; set; }
    [   ForeignKey("TweetId")]
        public Tweet? Tweet { get; set; }

        public int HashtagId { get; set; }
        [ForeignKey("HashtagId")]
        public Hashtag? Hashtag { get; set; }
    }
}
