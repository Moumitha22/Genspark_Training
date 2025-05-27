using System;

namespace Twitter.Models{

    public class Hashtag
    {
        public int Id { get; set; }
        public string Tag { get; set; } = string.Empty;

        public ICollection<TweetHashtag>? TweetHashtags { get; set; }
    }

}
