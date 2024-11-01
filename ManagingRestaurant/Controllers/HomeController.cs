using ManagingRestaurant.Data;
using ManagingRestaurant.Models;
using ManagingRestaurant.Models.Main;
﻿using ManagingRestaurant.Areas.Blogs.Models;
using ManagingRestaurant.Areas.News.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using X.PagedList;

namespace ManagingRestaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly RestaurantContext _context;
        public const int ITEM_PER_PAGE_BLOG = 6;
        public HomeController(ILogger<HomeController> logger, RestaurantContext context, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
			_context = context;
			_scopeFactory = scopeFactory;

		}
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Blog(string? searchString, int? page = 1)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'AppDbContext.News'  is null.");
            }
            var blogs = _context.Blogs.Where(n => n.IsActive).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE_BLOG);
            if (!string.IsNullOrEmpty(searchString))
            {
                blogs = _context.Blogs.Where(n => n.Title.Contains(searchString) && n.IsActive).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE_BLOG);
            }
            var model = new IndexBlogsModel
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE_BLOG,
                totalBlogs = await _context.Blogs.CountAsync(),
                blogs = blogs
            };
            return View(model);
        }

        public async Task<IActionResult> BlogDetail(string? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return RedirectToAction("Error404");
            }
            var blogs = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == Guid.Parse(id));
            if (blogs == null)
            {
                return RedirectToAction("Error404");
            }
            return View(blogs);
        }

        public async Task<IActionResult> News(string? searchString, int? page = 1)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'AppDbContext.News'  is null.");
            }
            var news = _context.News.Where(n => n.IsActive).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE_BLOG);
            if (!string.IsNullOrEmpty(searchString))
            {
                news = _context.News.Where(n => n.Title.Contains(searchString) && n.IsActive).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE_BLOG);
            }
            var model = new IndexNewsModel
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE_BLOG,
                totalNews = await _context.News.CountAsync(),
                news = news
            };
            return View(model);
        }

        public async Task<IActionResult> NewsDetail(string? id)
        {
            if (id == null || _context.News == null)
            {
                return RedirectToAction("Error404");
            }
            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == Guid.Parse(id));
            if (news == null)
            {
                return RedirectToAction("Error404");
            }
            return View(news);
        }

        //[HttpPost]
        //public IActionResult TakeBlogEmail(string email)
        //{
        //    if (string.IsNullOrEmpty(email)) {
        //        return BadRequest("Email is not valid");
        //    }

        //    // Kiem tra trong csdl
        //    if(_context.Contacts.Any(s => s.Email == email))
        //    {
        //        return Ok("Email existed");
        //    }

        //    // Luu vao csdl
        //    var newContact = new Contact { Email = email };
        //    _context.Contacts.Add(newContact);
        //    _context.SaveChanges();

        //    return Ok("We received your email. Please follow to take big sales");
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Table_Reservation()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		public async Task<IActionResult> Shop()
		{
			var pCateTask = GetCategoriesAsync();
			var productTask = GetProductsActiveAsync();

			await Task.WhenAll(pCateTask, productTask);

			var pCate = await pCateTask;
			var product = await productTask;
			var model = new IndexCate()
			{
				pCate = pCate,
				product = product
			};
			return View(model);
		}

		private async Task<List<Category>> GetCategoriesAsync()
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<RestaurantContext>();
				return await context.Categories.Where(c => c.IsActive).ToListAsync();
			}
		}
        
        private async Task<List<Product>> GetProductsActiveAsync()
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<RestaurantContext>();
				return await context.Products.Where(p => p.IsActive).ToListAsync();
			}
		}

		public async Task<IActionResult> DetailProduct(Guid id)
		{
			var product = await _context.Products.FindAsync(id);
			await _context.Entry(product).Collection(p => p.ProductImages).LoadAsync();
			await _context.Entry(product).Collection(p => p.Comments).Query().Include(c => c.AppUser).LoadAsync();
			await _context.Entry(product).Reference(p => p.Category).LoadAsync();
			if (product == null)
			{
				return RedirectToAction("Error404", "Home");
			}
			await _context.SaveChangesAsync();
			
			var model = new IndexDetail()
			{
				product = product,
				userId = await _context.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefaultAsync()
			};
			return View(model);
		}
	}
}
