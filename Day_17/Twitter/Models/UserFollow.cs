using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twitter.Models
{
    public class UserFollow
    {
        public int FollowerId { get; set; }
        [ForeignKey("FollowerId")]
        public User? Follower { get; set; }

        public int FolloweeId { get; set; }
        [ForeignKey("FolloweeId")]
        public User? Followee { get; set; }

        public DateTime FollowedAt { get; set; }
    }
}
