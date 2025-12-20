using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CORE.APP.Domain;
namespace APP.Domain;

public class Blog : Entity
{
    [Required]
    public int UserId { get; set; }

    [StringLength(100)]
    [Required]
    public string Title { get; set; }
    public string Content { get; set; }
    public decimal? Rating { get; set; }
    public DateTime? PublishDate { get; set; }

    public User User { get; set; }

    public List<BlogTag> BlogTags { get; set; } = new List<BlogTag>();

    [NotMapped]
    public List<int> TagIds
    {
        get => BlogTags.Select(gt => gt.TagId).ToList();
        set => BlogTags = value.Select(v => new BlogTag { TagId = v }).ToList();
    }
}
