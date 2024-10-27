using ManagingRestaurant.Data;
using ManagingRestaurant.Models;
using ManagingRestaurant.Models.Main;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace ManagingRestaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly RestaurantContext _context;
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

        public IActionResult Privacy()
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
