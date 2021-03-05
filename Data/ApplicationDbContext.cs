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
        public DbSet<Book> Books;
        public DbSet<Tag> Tags;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Chapter>()
                .HasKey(c => new {c.BookId, c.ChapterNum});
            builder.Entity<Rating>()
                .HasKey(c => new { c.ApplicationUserID, c.BookId });
            builder.Entity<Tag>()
                .HasKey(c => new { c.Id });
        }
    }
}
