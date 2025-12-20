using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using APP.Domain;
using CORE.APP.Models;

namespace APP.Models;

public class BlogRequest : Request
{
    [Required(ErrorMessage = "{0} is required!")]
    [DisplayName("User")]
    public int? UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    public string Content { get; set; }

    public decimal? Rating { get; set; }

    public DateTime? PublishDate { get; set; }

    [DisplayName("Tags")]
    public List<int> TagIds { get; set; } = new List<int>();
}