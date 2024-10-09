using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Role.Models;
using ManagingRestaurant.Models;
using X.PagedList;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.Role.Controllers
{
    [Area("Role")]
    [Authorize(Roles = $"{RoleName.Administrator}")]
    public class HomeController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RestaurantContext _context;
        
        public HomeController(RoleManager<IdentityRole> roleManager, RestaurantContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }
        
        [TempData]
        public string StatusMessage { get; set; }
        
        public const int ITEM_PER_PAGE = 5;
        public CreateRoleModel CreateRoleModel { get; set; } = default!;
        
        // GET: Role
        public async Task<IActionResult> Index(string? searchString ,int? page)
        {
            if(_context.Roles == null)
            {
                return Problem("Entity set 'AppDbContext.roles'  is null.");
            }

            var roles = _context.Roles.OrderBy(n => n.Name).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                roles = _context.Roles.Where(n => n.Name.Contains(searchString)).OrderBy(n => n.Name).ToList();
            }
            var roleModels = new List<RoleModel>();
            foreach (var role in roles)
            {
                var roleModel = new RoleModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Claims = (await _roleManager.GetClaimsAsync(role)).Select(c => $"{c.Type} = {c.Value}").ToList()
                };
                roleModels.Add(roleModel);
            }
            var model = new RoleIndexModel()
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalRoles = await _context.Roles.CountAsync(),
                roles = roleModels.ToPagedList(page ?? 1, ITEM_PER_PAGE)
            };
            return View(model);
            

            
        }
        
        // GET: Role/Create
        public ActionResult Create()
        {
            return View(CreateRoleModel);
        }
        
        // Post: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("RoleName")] CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                var oldRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
                if(oldRoles.Any(r => r.Name == model.RoleName))
                {
                    StatusMessage = "Error: Role already exists.";
                    return View(CreateRoleModel);
                }
                var newRole = new IdentityRole(model.RoleName);
                var result = await _roleManager.CreateAsync(newRole);
                if (result.Succeeded)
                {
                    StatusMessage = $"Success: Role <b>{model.RoleName}</b> created.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    StatusMessage = "Error: Role not created.";
                    return View(CreateRoleModel);
                }
            } else
            {
                StatusMessage = "Error: Invalid input.";
                return View(CreateRoleModel);
            }
        }

        // GET: Role/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                StatusMessage = "Error: Role not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditRoleModel()
            {
                Role = role,
                RoleClaims = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync()
            };
            return View(model);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                StatusMessage = "Error: Role not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditRoleModel()
            {
                Role = role,
                RoleClaims = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync()
            };
            return View(model);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                await _roleManager.DeleteAsync(role);
                StatusMessage = $"Success: Role <b>{role.Name}</b> deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        
    }
}