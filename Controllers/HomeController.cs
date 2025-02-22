﻿using CourseWork.Data;
using CourseWork.Models;
using CourseWork.Models.ViewModels;
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
        public IActionResult Contents(int bookId)
        {
            string[] titles = dbContext.Chapters
                .Where(c => c.BookId == bookId)
                .OrderBy(c => c.ChapterNum)
                .Select(c => c.Title)
                .ToArray();
            ViewBag.BookId = bookId;
            return View(titles);
        }
        public async Task<IActionResult> ReadBook(int bookId, int? chapterNum, int? userRating)
        {
            chapterNum = chapterNum ?? 1;
            Chapter chapter = dbContext.Chapters.FirstOrDefault(c => c.BookId == bookId && c.ChapterNum == chapterNum);            
            if (chapter == null) return NotFound();

            ApplicationUser user = null;
            if(HttpContext.User.Identity.IsAuthenticated) user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            ViewBag.UserRating = userRating ?? CheckUserRating(user, bookId);
            ViewBag.BookId = bookId;

            return View(new PageViewModel
            {
                CurrentPage = chapterNum.GetValueOrDefault(),
                TotalPages = dbContext.Chapters.Where(c => c.BookId == bookId).Count(),
                Liked = CheckUserLiked(user, chapter.Id),
                Title = chapter.Title,
                Text = GetHtmlFromChapter(chapter)
            });
        }
        [HttpPost]
        public async Task<JsonResult> RatingClick(int value, int bookId)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var request = HttpContext.Request;
            var rate = new Rating { ApplicationUserID = user.Id, BookId = bookId, Mark = value };
            dbContext.Ratings.Add(rate);
            dbContext.SaveChanges();
            return Json(value);
        }
        [HttpPost]
        public async Task<JsonResult> LikeClick(int bookId, int chapterNum)
        {
            Chapter chapter = dbContext.Chapters.FirstOrDefault(c => c.BookId == bookId && c.ChapterNum == chapterNum);
            return Json(await AddOrDeleteLike(chapter.Id));
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
            ViewBag.HighRatingBooks = SortAndSetAverageRating(books, SelectionSize);
            return View();
        }
        public IActionResult SelectGenre(string genre)
        {
            var foundBooks = dbContext.Books.Include(book => book.Ratings).Where(book => book.Genre == genre);
            foreach (var book in foundBooks) GetAverageRating(book.Ratings, book);
            ViewBag.FoundBooks = foundBooks;
            ViewBag.Genre = genre;
            return View();
        }
       
        [HttpPost]
        public IActionResult Search(string text)
        {
            if (text != null)
            {
                var foundBookIds = dbContext.Chapters.Where(c => EF.Functions.FreeText(c.Text, text) || c.Title == text).Select(c => c.BookId)
                .Union(dbContext.Comments.Where(c => EF.Functions.FreeText(c.Text, text)).Select(c => c.BookId))
                .Union(dbContext.Books.Where(c => c.Name == text).Select(c => c.Id)).Distinct();
                var foundBooks = dbContext.Books.Include(book => book.Ratings).Where(c => foundBookIds.Contains(c.Id));
                foreach (var book in foundBooks) GetAverageRating(book.Ratings, book);
                ViewBag.FoundBooks = foundBooks;
            }
            else ViewBag.FoundBooks = null;
            return View();
        }
        [HttpGet]
        public IActionResult Subscribe(int bookId)
        {
            ViewBag.BookId = bookId;
            return View();
        }
        [HttpPost]
        public IActionResult Subscribe(SubscribeViewModel model)
        {
            if (ModelState.IsValid)
            {
                Subscribe newSubscribe = new Subscribe { BookId = model.BookId, Email = model.Email };
                if (dbContext.Subscribes.Where(s => s.Email == newSubscribe.Email).Count() == 0)
                {
                    dbContext.Subscribes.Add(newSubscribe);
                    dbContext.SaveChanges();
                }            
            }
            return RedirectToAction("Index");
        }
        private int? CheckUserRating(ApplicationUser user, int bookId)
        {
            if (user == null) return null;
            var rating = dbContext.Ratings.Find(user.Id, bookId);
            return rating?.Mark;
        }
        private bool CheckUserLiked(ApplicationUser user, int chapterId)
        {
            if (user != null)
                if (dbContext.Likes.Find(chapterId, user.Id) != null) return true;
            return false;
        }
        private async Task<bool> AddOrDeleteLike(int chapterId)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var result = false;
            Like foundLike = dbContext.Likes.Find(chapterId, user.Id);
            if (foundLike != null) dbContext.Likes.Remove(foundLike); 
            else
            {
                Like newLike = new Like { ChapterId = chapterId, UserId = user.Id };
                dbContext.Likes.Add(newLike);
                result = true;
            }
            dbContext.SaveChanges();
            return result;
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
        private List<Book> SortAndSetAverageRating(List<Book> books, int selectionSize)
        {
            var sortedList = (from book in books orderby GetAverageRating(book.Ratings, book) descending select book)
                .Take(selectionSize)
                .ToList();
            return sortedList;
        }
        private float? GetAverageRating(List<Rating> ratings, Book book)
        {
            float? totalRating = 0;
            foreach (var rating in ratings)
                totalRating += rating.Mark;
            var count = ratings.Count;
            if (count == 0) totalRating = null;
            else totalRating /= count;
            book.AverageRating = totalRating;
            return totalRating;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private string GetHtmlFromChapter(Chapter chapter)
        {
            string result = $"<h3>{chapter.Title}</h3>";
            if (chapter.PicturePath != null)
                result += $"<img src=\"{StorageUtils.GetPictureUri(chapter.PicturePath)}\" alt=\"...\" style=\"max-height: 250px\">";
            result += MarkdownUtils.MarkdownParser(chapter.Text);
            return result;
        }
        private string GetBookAsAString(int bookId)
        {
            var book = dbContext.Books.FirstOrDefault(b => b.Id == bookId);
            if(book != null)
            {
                var chapters = dbContext.Chapters.Where(c => c.BookId == bookId);
                string res = $"<h1>{book.Name}</h1>";
                foreach (var chapter in chapters)
                {
                    res += GetHtmlFromChapter(chapter);
                }
                return res;
            }
            return null;
        }
        public IActionResult GetPdf(int bookId)
        {
            var renderer = new IronPdf.HtmlToPdf();
            var pdf = renderer.RenderHtmlAsPdf(GetBookAsAString(bookId)).BinaryData;
            return File(pdf, "application/pdf");
        }
    }
}
