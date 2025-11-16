using APP.Domain;
using CORE.APP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
    public class UserRequest : Request
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters!")]
        [DisplayName("Username")]
        public string UserName { get; set; }

        // Password is only required on Create. 
        // On Edit, a null/empty value means "don't change".
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} must be at least {2} characters!")]
        public string Password { get; set; }

        [StringLength(100)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [StringLength(100)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public Gender Gender { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [DisplayName("Country")]
        public int? CountryId { get; set; }

        [DisplayName("City")]
        public int? CityId { get; set; }

        [DisplayName("Group")]
        public int? GroupId { get; set; }

        [DisplayName("Roles")]
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}