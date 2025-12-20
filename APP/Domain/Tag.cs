using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace APP.Domain
{
    public class Tag : Entity
    {
        [Required, StringLength(125)]
        public string Name { get; set; }

        public List<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    }
}