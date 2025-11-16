using APP.Domain;
using CORE.APP.Models;
using System;
using System.ComponentModel;

namespace APP.Models
{
    public class UserResponse : Response
    {
        [DisplayName("Username")]
        public string UserName { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public Gender Gender { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Registered")]
        public DateTime RegistrationDate { get; set; }

        public decimal Score { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public string Address { get; set; }

        [DisplayName("Group")]
        public string GroupTitle { get; set; }

        [DisplayName("Roles")]
        public string Roles { get; set; }
    }
}