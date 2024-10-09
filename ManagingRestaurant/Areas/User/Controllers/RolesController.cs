using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Models;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = $"{RoleName.Administrator}")]
public class RolesController : Controller
{
    private readonly RestaurantContext _context;
    private readonly UserManager<AppUser> _userManager;
    [TempData] public string StatusMessage { get; set; }

    public RolesController(RestaurantContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // GET: User/Role/1
    public async Task<IActionResult> Roles(Guid? id)
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Roles(List<string> roles, Guid id)
    {
        if(_context.Users == null)
        {
            return Problem("Entity set 'AppDbContext.Users'  is null.");
        }
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.RemoveFromRolesAsync(user,await _userManager.GetRolesAsync(user));
            await _userManager.AddToRolesAsync(user,roles);
            StatusMessage = "Roles updated successfully.";
            
        }
        else
        {
            StatusMessage = "Error: User not found.";
        }
        return RedirectToAction("Edit","Home",new {id = id});
    }
}