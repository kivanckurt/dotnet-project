using CORE.APP.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Domain
{
    public class Role : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation Property: Many Roles have many Users (via UserRole)
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        [NotMapped]
        public List<int> UserIds
        {
            get => UserRoles.Select(ur => ur.UserId).ToList();
            set => UserRoles = value.Select(v => new UserRole() { UserId = v }).ToList();
        }
    }
}