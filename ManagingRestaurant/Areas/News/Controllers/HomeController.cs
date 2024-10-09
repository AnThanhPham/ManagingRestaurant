using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.News.Models;
using ManagingRestaurant.Models;
using X.PagedList;
using ManagingRestaurant.Data;
using ManagingRestaurant.Upload;

namespace ManagingRestaurant.Areas.News.Controllers
{
    [Area("News")]
    [Authorize(Roles = $"{RoleName.Administrator},{RoleName.Manager}")]
    public class HomeController : Controller
    {
        private readonly RestaurantContext _context;
        private readonly IBufferedFileUploadService _uploadService;
        public HomeController(RestaurantContext context, IBufferedFileUploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public const int ITEM_PER_PAGE = 5;

        // GET: News/Home
        public async Task<IActionResult> Index(string? searchString, int? page = 1)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'RestaurantContext.News'  is null.");
            }
            var news = _context.News.OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(searchString))
            {
                news = _context.News.Where(n => n.Title.Contains(searchString)).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            var model = new IndexNewsModel
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalNews = await _context.News.CountAsync(),
                news = news
            };
            return View(model);
        }


        // GET: News/Home/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Desciption,ShortDesc,IsActive")] ManagingRestaurant.Models.New news,
            IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string imageLink = await _uploadService.UploadFile(file);
                    if (!string.IsNullOrEmpty(imageLink))
                    {
                        news.Image = imageLink;
                    }
                }
                news.Id = Guid.NewGuid();
                news.CreatedAt = DateTime.Now;
                news.UpdatedAt = DateTime.Now;
                news.CreatedBy = User.Identity.Name;
                news.UpdatedBy = User.Identity.Name;
                _context.Add(news);
                StatusMessage = "News has been created.";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: News/Home/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile? file,
            [Bind("Id,Title,Desciption,ShortDesc,Image,IsActive,CreatedAt,CreatedBy")] ManagingRestaurant.Models.New news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        string imageLink = await _uploadService.UploadFile(file);
                        if (!string.IsNullOrEmpty(imageLink))
                        {
                            if (news.Image != null)
                            {
                                await _uploadService.DeleteFile(news.Image);
                            }
                            news.Image = imageLink;
                        }
                    }
                    news.UpdatedAt = DateTime.Now;
                    news.UpdatedBy = User.Identity.Name;
                    _context.Update(news);
                    StatusMessage = "News has been updated.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: News/Home/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'AppDbContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                if (news.Image != null)
                {
                    await _uploadService.DeleteFile(news.Image);
                }
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult DeleteMany()
        {
            return Json(new { success = true, message = "test" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody] string? data)
        {
            if (_context.News == null)
            {
                return Json(new { success = false, message = "Entity set 'AppDbContext.News'  is null." });
            }
            if (data == null)
            {
                return Json(new { success = false, message = "Data is null." });
            }
            var ids = data.Split(",");
            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out Guid guid))
                {
                    var news = await _context.News.FindAsync(guid);
                    if (news != null)
                    {
                        if (news.Image != null)
                        {
                            await _uploadService.DeleteFile(news.Image);
                        }
                        _context.News.Remove(news);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }
        private bool NewsExists(Guid id)
        {
            return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
