using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using ManagingRestaurant.Data;
using ManagingRestaurant.Upload;
using ManagingRestaurant.Areas.Blogs.Models;
using ManagingRestaurant.Models;

namespace ManagingRestaurant.Areas.Blogs.Controllers
{
    [Area("Blogs")]
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
            if(_context.Blogs == null)
            {
                return Problem("Entity set 'AppDbContext.Blogs'  is null.");
            }
            var blogs = _context.Blogs.OrderBy( n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(searchString))
            {
                blogs = _context.Blogs.Where(n => n.Title.Contains(searchString)).OrderBy(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            var model = new IndexBlogsModel
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalBlogs = await _context.Blogs.CountAsync(),
                blogs = blogs
            };
            return View(model);
        }
        

        // GET: Blogs/Home/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }

            return View(blogs);
        }

        // GET: Blogs/Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Desciption,ShortDesc,IsActive")] Blog blogs,
            IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string imageLink = await _uploadService.UploadFile(file);
                    if(!string.IsNullOrEmpty(imageLink))
                    {
                        blogs.Image = imageLink;
                    }
                }
                blogs.Id = Guid.NewGuid();
                blogs.CreatedAt = DateTime.Now;
                blogs.UpdatedAt = DateTime.Now;
                blogs.CreatedBy = User.Identity.Name;
                blogs.UpdatedBy = User.Identity.Name;
                _context.Add(blogs);
                StatusMessage = "Blogs has been created.";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogs);
        }

        // GET: Blogs/Home/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs == null)
            {
                return NotFound();
            }
            return View(blogs);
        }

        // POST: Blogs/Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,IFormFile? file,
            [Bind("Id,Title,Desciption,ShortDesc,Image,IsActive,CreatedAt,CreatedBy")] Blog blogs)
        {
            if (id != blogs.Id)
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
                        if(!string.IsNullOrEmpty(imageLink))
                        {
                            if(blogs.Image != null)
                            {
                               await _uploadService.DeleteFile(blogs.Image);
                            }
                            blogs.Image = imageLink;
                        }
                    }
                    blogs.UpdatedAt = DateTime.Now;
                    blogs.UpdatedBy = User.Identity.Name;
                    _context.Update(blogs);
                    StatusMessage = "Blogs has been updated.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogsExists(blogs.Id))
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
            return View(blogs);
        }

        // GET: Blogs/Home/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }

            return View(blogs);
        }

        // POST: Blogs/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'AppDbContext.Blogs'  is null.");
            }
            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs != null)
            {
                if(blogs.Image != null)
                {
                    await _uploadService.DeleteFile(blogs.Image);
                }
                _context.Blogs.Remove(blogs);
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
        public async Task<IActionResult> DeleteMany([FromBody]string? data)
        {
            if (_context.Blogs == null)
            {
                return Json(new { success = false, message = "Entity set 'AppDbContext.Blogs'  is null." });
            }
            if(data == null)
            {
                return Json(new { success = false, message = "Data is null." });
            }
            var ids = data.Split(",");
            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out Guid guid))
                {
                    var blogs = await _context.Blogs.FindAsync(guid);
                    if (blogs != null)
                    {
                        if(blogs.Image != null)
                        {
                            await _uploadService.DeleteFile(blogs.Image);
                        }
                        _context.Blogs.Remove(blogs);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }
        private bool BlogsExists(Guid id)
        {
          return (_context.Blogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
