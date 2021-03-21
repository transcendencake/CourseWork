using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CourseWork.Models;

namespace CourseWork.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Chapter>()
                .HasKey(c => new {c.Id});
            builder.Entity<Rating>()
                .HasKey(c => new { c.ApplicationUserID, c.BookId });
            builder.Entity<Book>()
                .HasMany(c => c.Tags)
                .WithMany(s => s.Books);
            builder.Entity<Tag>()
                .HasKey(c => new { c.Value });
            builder.Entity<Comment>()
                .HasKey(c => new { c.Id });
            builder.Entity<Like>()
                .HasKey(c => new { c.ChapterId, c.UserId });
       }
    }
}
