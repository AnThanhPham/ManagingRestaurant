using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Order.Models;
using ManagingRestaurant.Areas.Payment.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Services;
using NuGet.Packaging;
using X.PagedList;
using ManagingRestaurant.Data;
using ManagingRestaurant.Upload;

namespace ManagingRestaurant.Areas.Order.Controllers
{
    [Area("Order")]
    [Authorize(Roles = $"{RoleName.Administrator} , {RoleName.Manager}")]
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
        public async Task<IActionResult> Index(string? searchString,int? status, int? page = 1 )
        {
            var order = _context.Orders.OrderByDescending( n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (status != -1 && status != null)
            {
                order = _context.Orders.Where(o => o.Status == status).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                order = order.Where(n => n.Code.Contains(searchString)).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            
            var model = new IndeOrdersModel()
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalOrders = await _context.Orders.CountAsync(),
                orders = order,
                StatusOrder = (StatusOrder)status.GetValueOrDefault()
            };
            return View(model);
        }
        

        // GET: Orders/Home/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // GET: Orders/Home/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,UpdateOrderDto order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var findOrder = await _context.Orders.FindAsync(id);
                    if (findOrder == null)
                    {
                        return NotFound();
                    }
                    findOrder.Code = order.Code;
                    findOrder.AppUserId = order.AppUserIdFK;
                    findOrder.CustomerName = order.CustomerName;
                    findOrder.CustomerEmail = order.CustomerEmail;
                    findOrder.PhoneNumber = order.PhoneNumber;
                    findOrder.Address = order.Address;
                    findOrder.TotalAmount = order.TotalAmount;
                    findOrder.Quantity = order.Quantity;
                    findOrder.MethodPay = (int)order.MethodPay;
                    findOrder.IsConfirmByUser = order.IsConfirmByUser;
                    findOrder.IsConfirmByShop = order.IsConfirmByShop;
                    findOrder.Status = (int)order.Status;
                    findOrder.UpdatedAt = DateTime.Now;
                    findOrder.UpdatedBy = User.Identity.Name;
                    _context.Orders.Update(findOrder);
                    StatusMessage = "Orders has been updated.";

                    if (findOrder.Status == (int)StatusOrder.Completed)
                    {
                        var findStatics = await _context.Statisticals.FirstOrDefaultAsync(
                            s => s.Time == DateTime.Now
                            );
                        if (findStatics == null)
                        {
                            var newStatics = new Statistical()
                            {
                                Time = DateTime.Now,
                                TotalAmount = order.TotalAmount,
                                TotalQuantity = order.Quantity,
                                TotalOrderComplicated = 1,
                                
                            };
                            await _context.Statisticals.AddAsync(newStatics);
                        }
                        else
                        {
                            findStatics.TotalAmount += order.TotalAmount;
                            findStatics.TotalOrderComplicated += 1;
                            findStatics.TotalQuantity += order.Quantity;
                        }
                    }
                    
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExisted(order.Id))
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
            var orderModel = await _context.Orders.FindAsync(id);
            return View(orderModel);
        }

        // GET: Orders/Home/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            StatusMessage = "Orders has been deleted.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        
        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody]string? data)
        {
            if (_context.Orders == null)
            {
                return Json(new { success = false, message = "Entity set 'AppDbContext.Orders'  is null." });
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
                    var order = await _context.Orders.FindAsync(guid);
                    if (order != null)
                    {
                        _context.Orders.Remove(order);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delete success." });
        }
        private bool OrderExisted(Guid id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
