using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace CourseWork.Controllers
{
    public class HomeController : Controller
    {
        private const int SelectionSize = 5;

        private readonly ILogger<HomeController> _logger;
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            _logger = logger;
            dbContext = applicationDbContext;
        }          
        public async Task<IActionResult> ReadBook(int bookId, int? chapterNum)
        {
            chapterNum = chapterNum ?? 1;
            Chapter chapter = dbContext.Chapters.FirstOrDefault(c => c.BookId == bookId && c.ChapterNum == chapterNum);            
            if (chapter == null) return NotFound();
            PageViewModel model = new PageViewModel
            {
                CurrentPage = chapterNum.GetValueOrDefault(),
                TotalPages = dbContext.Chapters.Where(c => c.BookId == bookId).Count(),
                Liked = await CheckUserLiked(chapter.Id),
                Text = MarkdownUtils.MarkdownParser(chapter.Text)
            };
            ViewBag.BookId = bookId;
            return View(model);
        }
        private async Task<bool> CheckUserLiked(int chapterId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (user != null)
                    if (dbContext.Likes.Find(chapterId, user.Id) != null) return true;
            }
            return false;
        }
        public IActionResult Index(string tags)
        {
            var books = dbContext.Books
                .Include(book => book.Ratings)
                .Include(book => book.Tags)
                .ToList();
            if (tags != null)
            {
                ViewBag.ContainTags = SortByTags(books, tags, SelectionSize);
            }
            else ViewBag.ContainTags = null;
            ViewBag.Tags = String.Join(',', dbContext.Tags.Select(tag => tag.Value));
            ViewBag.LastUpdateBooks = SortByUpdateDate(books, SelectionSize);
            ViewBag.HighRatingBooks = SortByAverageRating(books, SelectionSize);
            return View();
        }
        private List<Book> SortByTags(List<Book> books, string tags, int selectionSize)
        {
            var sortedList = books.Where(book => TagsUtils.NormalizeTags(tags).All(tag => book.Tags.Find(x => x.Value == tag) != null))
                .Take(selectionSize)
                .ToList();
            return sortedList;
        }
        private List<Book> SortByUpdateDate(List<Book> books, int selectionSize)
        {
            var sortedList = (from book in books orderby book.UpdateDate descending select book)
                .Take(selectionSize)
                .ToList();
            return sortedList;
        }
        private List<Book> SortByAverageRating(List<Book> books, int selectionSize)
        {
            var sortedList = (from book in books orderby GetAverageRating(book.Ratings) descending select book)
                .Take(selectionSize)
                .ToList();
            return sortedList;
        }
        private float GetAverageRating(List<Rating> ratings)
        {
            float totalRating = 0;
            foreach (var rating in ratings)
            {
                totalRating += rating.Mark;
            }
            return totalRating / ratings.Count;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
