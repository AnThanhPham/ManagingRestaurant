using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.User.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = $"{RoleName.Administrator}")]
public class SetPasswordController : Controller
{
    private readonly RestaurantContext _context;
    private readonly UserManager<AppUser> _userManager;
    [TempData] public string StatusMessage { get; set; }

    public SetPasswordController(RestaurantContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [BindProperty]
    public SetPassModel model {set; get;}
    
    // GET: User/SetPassword/asdd-asdjv-asd
    public async Task<IActionResult> SetPassword(Guid? id)
    {
        if (id == null || _context.Users == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id.ToString());
        
        model = new SetPassModel
        {
            Id = user.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            Image = string.IsNullOrEmpty(user.Image) ? "https://www.pngkey.com/png/full/72-729716_user-avatar-png-graphic-free-download-icon.png" : user.Image,
            Address = user.Address,
        };
        if (user == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(
        [Bind("Id,Password,UserName,FullName,Email,Image,Address,Job,ConfirmedPassword")]
        SetPassModel setPassModel)
    {
        if (ModelState.IsValid)
        {
            
            var user = await _userManager.FindByIdAsync(setPassModel.Id);
            if (user != null)
            {
                var deletePass = user.PasswordHash != null ? await _userManager.RemovePasswordAsync(user) : IdentityResult.Success;
                if (deletePass.Succeeded)
                {
                    var addPass = await _userManager.AddPasswordAsync(user, setPassModel.Password);
                    if (addPass.Succeeded)
                    {
                        StatusMessage = "Password updated successfully.";
                        
                    }
                    else
                    {
                        StatusMessage = "Error: Password not updated.";
                    }
                    return RedirectToAction("Edit","Home",new {id = setPassModel.Id});
                }
            }
        }
        StatusMessage = "Error: Password not updated.";
        return View(setPassModel);
    }
}