using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CourseWork.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Rating> Ratings { get; set; }
        public List<Book> Books { get; set; }
    }
}
