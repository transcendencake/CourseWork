using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class Rating
    {
        public string ApplicationUserID { get; set; }
        public int BookId { get; set; }
        [Range(0, 5)]
        public int Mark { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Book Book { get; set; }
    }

    public enum Genres
    {
        //not implemented
    }

    public class Book
    {
        public string ApplicationUserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Rating> Ratings { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<Chapter> Chapters { get; set; }
    }
    public class Tag
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Value { get; set; }
        public Book Book { get; set; }
    }
    public class Chapter
    {
        public int BookId { get; set; }
        public int ChapterNum { get; set; }
        public string Text { get; set; }
        public Book Book { get; set; }
    }
}
