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
using Microsoft.AspNetCore.Mvc.Rendering;

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
        [Authorize]
        [HttpGet]
        public IActionResult AddBook(string userId)
        {
            ViewBag.UserId = userId;
            ViewBag.Tags = String.Join(',', dbContext.Tags.Select(tag => tag.Value));
            ViewBag.Genres = new SelectList(Enum.GetNames(typeof(Genres)));
            return View();
        }
        [HttpPost]
        public IActionResult AddBook(BookAddViewModel model)
        {
            model.Book.UpdateDate = DateTime.Now;
            model.Book.UploadDate = DateTime.Now;
            dbContext.Books.Add(model.Book);

            Logger.DebugLogger.LogDebug(model.Tags);

            model.Book.Tags.AddRange(GetTagsToAdd(model.Tags));

            AddChapter(model.Book, 1, null);

            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var book = dbContext.Books.Find(bookId);
            if (book == null) return NotFound();

            if (await CheckBookAuthority(book.ApplicationUserId))
            {
                dbContext.Books.Remove(book);
                dbContext.SaveChanges();
            }                 

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditBook(int bookId, int? chapterNum = null)
        {
            var book = dbContext.Books.Find(bookId);
            if(await CheckBookAuthority(book.ApplicationUserId))
            {
                ViewBag.BookId = bookId;
                ViewBag.Chapters = dbContext.Chapters.Where(chapter => chapter.BookId == book.Id).Count();
                Chapter chapter = null;
                if (chapterNum != null)
                {
                    chapter = dbContext.Chapters.Find( book.Id, chapterNum);
                }

                return View(chapter);
            }
            else
            {
                return NotFound();
            }            
        }

        [HttpPost]
        public JsonResult ChangeChaptersOrder(int was, int become)
        {
            //not implemented
            string result = was.ToString() + "   " + become.ToString();
            return Json(result);
        }

        private void AddChapter(Book book, int chapterNum, string text)
        {
            var chapter = new Chapter { Book = book, ChapterNum = chapterNum, Text = text, BookId = book.Id };
            dbContext.Chapters.Add(chapter);
            dbContext.SaveChanges();
        }

        private async Task<bool> CheckBookAuthority(string bookAuthorId)
        {           
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.IsInRole("admin") || user.Id == bookAuthorId)
                return true;
            else return false;
        }
        private List<Tag> GetTagsToAdd(string inputTags)
        {
            var inputTextArr = NormalizeTags(inputTags);
            var result = new List<Tag>();
            foreach (var tag in inputTextArr)
            {
                Tag foundTag = dbContext.Tags.Find(tag);
                if (foundTag != null) result.Add(foundTag);
                else
                {
                    var newTag = new Tag { Value = tag };
                    result.Add(newTag);
                    dbContext.Tags.Add(newTag);                    
                }
            }
            dbContext.SaveChanges();
            return result;
        }
        private string[] NormalizeTags(string tags)
        {
            string[] tagsArr = tags?.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int length = tagsArr == null ? 0 : tagsArr.Length;
            var result = new string[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = tagsArr[i].Split('"')[3];
            }
            return result;
        }

    }
}
