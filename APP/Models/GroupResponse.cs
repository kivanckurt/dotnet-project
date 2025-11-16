using CORE.APP.Models;
using System.ComponentModel;

namespace APP.Models
{
    public class GroupResponse : Response
    {
        public string Title { get; set; }

        [DisplayName("Users")]
        public string UserNames { get; set; } // Added this property
    }
}