using CORE.APP.Models;
using System;
using System.ComponentModel;

namespace APP.Models
{
    public class BlogResponse : Response
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [DisplayName("Rating")]
        public decimal? Rating { get; set; }
        [DisplayName("Publish Date")]
        public DateTime? PublishDate { get; set; }
        [DisplayName("Publish Date")]
        public string PublishDateF { get; set; }
        [DisplayName("Rating")]
        public string RatingF { get; set; }

        public string User { get; set; }

        public UserResponse UserResponse { get; set; }

        public List<int> TagIds { get; set; }
        public string Tags { get; set; }
        public List<TagResponse> TagsResponse { get; set; }
    }
}