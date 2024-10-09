using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.User.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Upload;
using X.PagedList;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = $"{RoleName.Administrator},{RoleName.Manager}")]
    public class HomeController : Controller
    {
        private readonly RestaurantContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBufferedFileUploadService _bufferedFileUploadService;
        
        public string ImageDefault = "https://www.pngkey.com/png/full/72-729716_user-avatar-png-graphic-free-download-icon.png";

        public HomeController(RestaurantContext context, UserManager<AppUser> userManager, IBufferedFileUploadService bufferedFileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _bufferedFileUploadService = bufferedFileUploadService;
        }
        [TempData]
        public string StatusMessage { get; set; }
        public const int ITEM_PER_PAGE = 5;

        // GET: User/
        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            if(_context.Users == null)
            {
                return Problem("Entity set 'AppDbContext.users'  is null.");
            }
            var users = _context.Users.OrderBy( n => n.UserName).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(searchString))
            {
                users = _context.Users.Where(n => n.UserName.Contains(searchString)).OrderBy(n => n.UserName).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            var model = new IndexModel
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalUsers = await _context.Users.CountAsync(),
                users = users
            };
            return View(model);
        }

        // GET: User/User/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,UserName,FullName,Email,PhoneNumber" +
                  ",Image,Address,Birthday,EmailConfirmed,PhoneNumberConfirmed")] 
            AppUser user, string password, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                var admin = await _userManager.GetUserAsync(User);
                user.Created_at = DateTime.Now;
                user.Update_at = DateTime.Now;
                user.Created_by = admin.UserName;
                user.Update_by = admin.UserName;
                user.FullName = "Test";
                user.Image = user.Image == null ? ImageDefault : user.Image;
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user,roles);
                    StatusMessage = "User created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }

        // GET: User/User/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile? file,
            [Bind("Id,UserName,FullName,Email,PhoneNumber,Image,Address,Birthday,EmailConfirmed,PhoneNumberConfirmed")] 
            AppUser user)
        {
            if (id.ToString() != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _userManager.FindByIdAsync(user.Id);
                    if (userToUpdate != null)
                    {
                        if(file != null)
                        {
                            string imageLink = await _bufferedFileUploadService.UploadFile(file);
                            if(!string.IsNullOrEmpty(imageLink))
                            {
                                userToUpdate.Image = imageLink;
                            }
                        }
                        
                        userToUpdate.UserName = user.UserName;
                        userToUpdate.Email = user.Email;
                        userToUpdate.PhoneNumber = user.PhoneNumber;
                        userToUpdate.Address = user.Address;
                        userToUpdate.FullName = user.FullName;
                        userToUpdate.Birthday = user.Birthday;
                        userToUpdate.Update_at = DateTime.Now;
                        userToUpdate.Update_by = User.Identity?.Name;
                        userToUpdate.EmailConfirmed = user.EmailConfirmed;
                        userToUpdate.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                        await _context.SaveChangesAsync();
                        StatusMessage = "User updated successfully.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: User/User/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'AppDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id.ToString());
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user == null)
            {
                StatusMessage = "Error: User not found.";
                return RedirectToAction(nameof(Index));
            }
            if(user.Image != null && user.Image != ImageDefault)
            {
                user.Image = ImageDefault;
                user.Update_at = DateTime.Now;
                user.Update_by = User.Identity?.Name;
                await _context.SaveChangesAsync();
                StatusMessage = "Image deleted successfully.";
            }
            return RedirectToAction("Edit", new {id = id});
        }
        
        private bool UserExists(string id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
