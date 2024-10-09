using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagingRestaurant.Areas.Product.Models;
using ManagingRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Product.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Services;
using NuGet.Packaging;
using X.PagedList;
using ManagingRestaurant.Data;
using ManagingRestaurant.Upload;

namespace ManagingRestaurant.Areas.Product.Controllers
{
    [Area("Product")]
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
            var product = _context.Products.OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(searchString))
            {
                product = _context.Products.Where(n => n.Name.Contains(searchString)).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }

            foreach (var p in product)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == p.CategoryId);
                if (category != null)
                {
                    p.Category = category;
                }
            }
            var model = new IndexProductsModel
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalProducts = await _context.Products.CountAsync(),
                products = product
            };
            return View(model);
        }


        // GET: Products/Home/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            var productImages = await _context.ProductImages.Where(p => p.ProductId == product.Id).ToListAsync();
            if (category != null)
            {
                product.Category = category;
            }
            if (productImages.Count > 0)
            {
                product.ProductImages = productImages;
            }


            return View(product);
        }

        // GET: Products/Home/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Title");
            return View();
        }

        // POST: Products/Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile? file, IFormFile[]? files,
            [Bind("Id,Name,ShortDesc,ProductCode,Description,Price,"+
                  "Quantity," +
                  "IsActive,CategoryId")] ManagingRestaurant.Models.Product product)
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Title");
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string imageLink = await _uploadService.UploadFile(file);
                    if (!string.IsNullOrEmpty(imageLink))
                    {
                        product.Image = imageLink;
                    }
                }

                if (files != null)
                {
                    var imageLinks = await _uploadService.UploadManyFiles(files);
                    if (imageLinks.Count > 0)
                    {
                        var productImages = new List<ProductImage>();
                        foreach (var img in imageLinks)
                        {
                            productImages.Add(new ProductImage
                            {
                                Id = Guid.NewGuid(),
                                Image = img,
                                ProductId = product.Id,
                                IsDefault = false
                            });
                        }
                        product.ProductImages = productImages;
                    }
                }

                product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;
                product.CreatedBy = User.Identity.Name;
                product.UpdatedBy = User.Identity.Name;
                product.Category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
                _context.Add(product);
                StatusMessage = "Products has been created.";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Home/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            var productImages = await _context.ProductImages.Where(p => p.ProductId == product.Id).ToListAsync();
            if (category != null)
            {
                product.Category = category;
            }
            if (productImages.Count > 0)
            {
                product.ProductImages = productImages;
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Title", category.Id.ToString());
            return View(product);
        }

        // POST: Products/Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile? file, IFormFile[]? files,
            [Bind("Id,Name,Description,ProductCode,ShortDesc,Price,Image,"+
            "Quantity,CreatedAt,CreatedBy," +
            "IsActive,ProductImages,CategoryId,")] ManagingRestaurant.Models.Product product)
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Title");
            if (id != product.Id)
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
                            if (product.Image != null)
                            {
                                await _uploadService.DeleteFile(product.Image);
                            }
                            product.Image = imageLink;
                        }
                    }

                    if (files != null)
                    {
                        var imageLinks = await _uploadService.UploadManyFiles(files);
                        if (imageLinks.Count > 0)
                        {
                            var productImages = new List<ProductImage>();
                            foreach (var img in imageLinks)
                            {
                                productImages.Add(new ProductImage
                                {
                                    Id = Guid.NewGuid(),
                                    Image = img,
                                    ProductId = product.Id,
                                    IsDefault = false
                                });
                            }

                            await _context.ProductImages.AddRangeAsync(productImages);
                        }
                    }


                    product.UpdatedAt = DateTime.Now;
                    product.UpdatedBy = User.Identity.Name;
                    _context.Products.Update(product);
                    StatusMessage = "Products has been updated.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Home/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (product.Image != null)
                {
                    await _uploadService.DeleteFile(product.Image);
                }
                var productImages = await _context.ProductImages.Where(p => p.ProductId == product.Id).ToListAsync();
                if (productImages.Count > 0)
                {
                    var listImage = productImages.Select(i => i.Image).ToList();
                    if (listImage.Count > 0)
                    {
                        await _uploadService.DeleteManyFiles(listImage);
                    }
                    _context.ProductImages.RemoveRange(productImages);
                }
                _context.Products.Remove(product);
            }
            StatusMessage = "Products has been deleted.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody] string? data)
        {
            if (_context.Products == null)
            {
                return Json(new { success = false, message = "Entity set 'AppDbContext.Products'  is null." });
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
                    var product = await _context.Products.FindAsync(guid);
                    if (product != null)
                    {
                        if (product.Image != null)
                        {
                            await _uploadService.DeleteFile(product.Image);
                        }
                        _context.Products.Remove(product);

                        var productImages = await _context.ProductImages.Where(p => p.ProductId == product.Id).ToListAsync();
                        if (productImages.Count > 0)
                        {
                            var listImage = productImages.Select(i => i.Image).ToList();
                            if (listImage.Count > 0)
                            {
                                await _uploadService.DeleteManyFiles(listImage);
                            }
                            _context.ProductImages.RemoveRange(productImages);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }
        private bool ProductsExists(Guid id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}