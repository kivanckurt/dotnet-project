using CORE.APP.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace APP.Domain
{
    public enum Gender
    {
        NotSpecified,
        Male,
        Female,
        Other
    }

    public class User : Entity
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; } // NOTE: This must be hashed by the service!

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime RegistrationDate { get; set; }

        public decimal Score { get; set; }

        public bool IsActive { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        // --- Foreign Keys ---

        public int? CountryId { get; set; }

        public int? CityId { get; set; }
        public int? GroupId { get; set; }

        public Group Group { get; set; }

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Helper property to manage Role IDs, just like your Product/Store example.
        /// </summary>
        [NotMapped]
        public List<int> RoleIds
        {
            get => UserRoles.Select(ur => ur.RoleId).ToList();
            set => UserRoles = value.Select(v => new UserRole() { UserId = this.Id, RoleId = v }).ToList();
        }
    }
}