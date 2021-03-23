using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models.ViewModels
{
    public class SubscribeViewModel
    {
        public int BookId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
