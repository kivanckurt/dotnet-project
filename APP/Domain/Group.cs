using CORE.APP.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APP.Domain
{
    public class Group : Entity
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        // Navigation Property: One Group has many Users
        public List<User> Users { get; set; } = new List<User>();
    }
}