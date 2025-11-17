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

        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [DisplayName("Score")]
        public decimal Score { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        [DisplayName("Group")]
        public string GroupTitle { get; set; }

        [DisplayName("Roles")]
        public string Roles { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [DisplayName("Gender")]
        public string GenderF { get; set; }

        [DisplayName("Birth Date")]
        public string BirthDateF { get; set; }

        [DisplayName("Registration Date")]
        public string RegistrationDateF { get; set; }

        [DisplayName("Score")]
        public string ScoreF { get; set; }

        [DisplayName("Active")]
        public string IsActiveF { get; set; }
    }
}