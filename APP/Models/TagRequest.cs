using CORE.APP.Models;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class TagRequest : Request
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(125, MinimumLength = 3, ErrorMessage = "{0} must be minimum {2} maximum {1} characters!")]
        public string Name { get; set; }
    }
}