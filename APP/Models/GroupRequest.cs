using CORE.APP.Models;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class GroupRequest : Request
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
    }
}