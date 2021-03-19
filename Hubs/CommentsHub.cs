using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CourseWork.Hubs
{
    public class CommentsHub : Hub
    {
        UserManager<ApplicationUser> userManager;
        ApplicationDbContext dbContext;
        public CommentsHub(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.dbContext = applicationDbContext;
        }
        public async Task NewComment (string text, int bookId, string userName)
        {
            ApplicationUser user = await userManager.FindByNameAsync(userName);
            if (user != null && Regex.IsMatch(text, @"^[^\|;]+$"))
            {
                Comment newComment = new Comment { Author = userName, BookId = bookId, Text = text, DateTime = DateTime.Now };
                dbContext.Comments.Add(newComment);
                dbContext.SaveChanges();
                await Clients.All.SendAsync("ReceiveComment", bookId, userName, text);
            }
        }
        public async Task GetAllComments(int bookId)
        {
            string comments = GetComments(bookId);
            await Clients.Caller.SendAsync("ReceiveAllComments", comments);
        }
        private string GetComments(int bookId)
        {
            return String.Join(';', dbContext.Comments
                .Where(c => c.BookId == bookId)
                .OrderBy(c => c.DateTime)
                .Select(c =>   c.Author.ToString() + "|" + c.Text ));
        }
    }
}
