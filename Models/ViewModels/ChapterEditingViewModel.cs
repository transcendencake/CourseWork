using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork.Models
{
    public class ChapterEditingViewModel
    {
        public Chapter Chapter { get; set; }
        public IFormFile Picture { get; set; }
    }
}
