using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Models
{
    public class UserManagerViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public static List<IdentityRole> AllRoles;

        static UserManagerViewModel()
        {

        }
        public UserManagerViewModel()
        {
            Roles = new List<string>();
        }
    }
}
