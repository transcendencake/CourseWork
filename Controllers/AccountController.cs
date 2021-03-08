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
using Microsoft.EntityFrameworkCore;

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

            return RedirectToAction("Index", new { userId = model.Book.ApplicationUserId});
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
            if (await CheckBookAuthority(book.ApplicationUserId))
            {
                book.UpdateDate = DateTime.Now;
                ViewBag.BookId = bookId;
                ViewBag.Chapters = dbContext.Chapters.Where(chapter => chapter.BookId == book.Id).Count();
                Chapter chapter = null;
                if (chapterNum != null)
                {
                    chapter = dbContext.Chapters.FirstOrDefault(chapter => chapter.BookId == book.Id && chapter.ChapterNum == chapterNum);
                }

                return View(chapter);
            }
            else
            {
                return NotFound();
            }            
        }
        [HttpPost]
        public IActionResult EditBook(Chapter model)
        {
            Chapter chapter = dbContext.Chapters.Find(model.Id);
            if (chapter != null)
            {
                chapter.Text = model.Text;
                dbContext.SaveChanges();
                return RedirectToAction("EditBook", new { bookId = chapter.BookId });
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> NewChapter(int bookId)
        {
            var book = dbContext.Books.Find(bookId);
            if (await CheckBookAuthority(book.ApplicationUserId))
            {
                var newChapterNum = dbContext.Chapters.Where(chapter => chapter.BookId == book.Id).Count() + 1;
                AddChapter(book, newChapterNum, null);

                return RedirectToAction("EditBook", new { bookId = bookId, chapterNum = newChapterNum});
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteChapter(int bookId, int chapterId)
        {
            var book = dbContext.Books.Find(bookId);
            if (await CheckBookAuthority(book.ApplicationUserId))
            {
                DeleteAndReorder(bookId, chapterId);

                return RedirectToAction("EditBook", new { bookId = bookId});
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public JsonResult ChangeChaptersOrder(int bookId, int was, int become)
        {
             string result = bookId.ToString() + "  " + was.ToString() + "   " + become.ToString();
            if (was == become)
            {
                return Json(result);
            }
            else
            {
                Chapter[] chapters = dbContext.Chapters.Where(c => c.BookId == bookId).OrderBy(c => c.ChapterNum).ToArray();
                if (was < become)
                {
                    for (int i = was + 1; i <= become; i++)
                    {
                        chapters[i].ChapterNum--;
                    }                    
                }
                else
                {
                    for (int i = become; i < was; i++)
                    {
                        chapters[i].ChapterNum++;
                    }
                }
                chapters[was].ChapterNum = become + 1;
                dbContext.SaveChanges();
            }            
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> EditTitle(int bookId)
        {
            var book = dbContext.Books.Include(book => book.Tags).FirstOrDefault(book => book.Id == bookId);
            if (await CheckBookAuthority(book.ApplicationUserId))
            {
                ViewBag.Tags = String.Join(',', dbContext.Tags.Select(tag => tag.Value));
                ViewBag.Genres = new SelectList(Enum.GetNames(typeof(Genres)));
            }
            else
            {
                return NotFound();
            }
            return View(new BookAddViewModel { Book = book });
        }
        [HttpPost]
        public IActionResult EditTitle(BookAddViewModel model)
        {            
            var book = dbContext.Books.Include(book => book.Tags).FirstOrDefault(book => book.Id == model.Book.Id);
            book.Name = model.Book.Name;
            book.UpdateDate = DateTime.Now;
            book.Genre = model.Book.Genre;
            book.Description = model.Book.Description;

            EditBookTags(book, book.Tags, model.Tags);

            dbContext.SaveChanges();

            return RedirectToAction("Index", new { userId = model.Book.ApplicationUserId });
        }
        private void DeleteAndReorder(int bookId, int chapterId)
        {
            var chapter = dbContext.Chapters.Find(chapterId);            
            if (chapter != null)
            {
                int chapterNum = chapter.ChapterNum;
                dbContext.Chapters.Remove(chapter);
                Chapter[] chapters = dbContext.Chapters.Where(c => c.BookId == bookId).ToArray();
                for (int i = 0; i < chapters.Length; i++)
                {
                    if (chapters[i].ChapterNum > chapterNum) chapters[i].ChapterNum--;
                }
                dbContext.SaveChanges();
            }                
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
            var inputTextArr = TagsUtils.NormalizeTags(inputTags);
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
        private void EditBookTags(Book book, List<Tag> was, string become)
        {
            var becomeTags = GetTagsToAdd(become);
            List<Tag> newTags = becomeTags.Except(was).ToList();
            List<Tag> tagsToDelete = was.Except(becomeTags).ToList();

            book.Tags.AddRange(newTags);
            book.Tags.RemoveAll(tag => tagsToDelete.Find(c => c.Value == tag.Value) != null);
        }
    }
}
