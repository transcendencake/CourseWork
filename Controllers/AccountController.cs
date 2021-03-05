using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CourseWork.Models;
using Microsoft.Extensions.Logging;
using CourseWork.Data;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext dbContext;
        private UserManager<ApplicationUser> userManager;
        public AccountController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            dbContext = applicationDbContext;
            this.userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string userId = null)
        {
            ApplicationUser user;
            if (HttpContext.User.IsInRole("admin") && userId != null)
            {
                user = await userManager.FindByIdAsync(userId);
            }
            else
            {
                user = await userManager.GetUserAsync(HttpContext.User);
                userId = user.Id;
            }
            var books = dbContext.Books.Where(book => book.ApplicationUserId == userId).ToList();
            ViewBag.UserId = userId;
            return View(books);
        }
        [HttpGet]
        public IActionResult AddBook()
        {

            return View();
        }
    }
}
