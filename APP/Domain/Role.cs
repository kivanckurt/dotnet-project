using CORE.APP.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APP.Domain
{
    public class Role : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation Property: Many Roles have many Users (via UserRole)
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}