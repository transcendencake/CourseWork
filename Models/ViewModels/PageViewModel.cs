using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork.Models
{
    public class PageViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Text { get; set; }
        public bool Liked { get; set; }
        public bool HasPrevPage
        {
            get { return CurrentPage > 1; }
        }
        public bool HasNextPage
        {
            get { return CurrentPage < TotalPages; }
        }
    }
}
