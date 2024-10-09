using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Models;
using X.PagedList;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.Contact
{
    [Area("Contact")]
    [Authorize(Roles = $"{RoleName.Administrator},{RoleName.Manager}")]
    public class HomeController : Controller
    {
        private readonly RestaurantContext _context;
        public HomeController(RestaurantContext context)
        {
            _context = context;
        }
        
        [TempData]
        public string StatusMessage { get; set; }
        
        public const int ITEM_PER_PAGE = 5;
        
        // GET: Contact/Contact
        public async Task<IActionResult> Index(string? searchString, int? page)
        {
            if(_context.Contacts == null)
            {
                return Problem("Entity set 'AppDbContext.contacts'  is null.");
            }
            var contacts = _context.Contacts.OrderByDescending( n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (!string.IsNullOrEmpty(searchString))
            {
                contacts = _context.Contacts.Where(n => n.UserName.Contains(searchString)).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            var model = new IndexContactModel()
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalContacts = await _context.Contacts.CountAsync(),
                contacts = contacts
            };
            return View(model);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Okok()
        {
            return View();
        }

        // GET: Contact/Contact/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/Contact/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contact/Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]BodyContactModel data)
        {
            if (data != null && ModelState.IsValid)
            {
                var contact = new Models.Contact();
                contact.Id = Guid.NewGuid();
                contact.UserName = data.UserName;
                contact.Email = data.Email;
                contact.PhoneNumber = data.PhoneNumber;
                contact.Message = data.Message;
                contact.CreatedAt = DateTime.Now;
                await _context.Contacts.AddAsync(contact);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Contact sended success!" });
            }
            return Json(new { success = false, message = "Invalid data request!" });
        }

        // GET: Contact/Contact/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            
            return View(contact);
        }

        // POST: Contact/Contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, UserName,Email,PhoneNumber,Message,CreatedAt,CreatedBy")] Models.Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                    StatusMessage = "Contact updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            return View(contact);
        }

        // GET: Contact/Contact/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'AppDbContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }
            
            await _context.SaveChangesAsync();
            StatusMessage = "Contact deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody]string? data)
        {
            if (_context.News == null)
            {
                return Json(new { success = false, message = "Entity set 'AppDbContext.Contacts'  is null." });
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
                    var news = await _context.Contacts.FindAsync(guid);
                    if (news != null)
                    {
                        _context.Contacts.Remove(news);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }

        private bool ContactExists(Guid id)
        {
          return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
