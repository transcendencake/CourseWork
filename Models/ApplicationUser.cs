using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Rating> Ratings { get; set; }
        public List<Book> Books { get; set; }
    }
}
