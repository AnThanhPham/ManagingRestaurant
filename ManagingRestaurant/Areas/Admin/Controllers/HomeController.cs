using ManagingRestaurant.Data;
using ManagingRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ManagingRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleName.Administrator)]
    public class HomeController : Controller
    {
        private readonly RestaurantContext _context;
        public HomeController(RestaurantContext context)
        {
            _context = context;
        }

        public IActionResult IndexHome()
        {
            /*var orders = await _context.Orders.ToListAsync();
            if (orders == null)
            {
                orders = new List<NetMVC.Models.Order>();
            }
            var totalRevenue = orders.Where(o => o.Status == (int)StatusOrder.Completed).Sum(o => o.TotalAmount);
            var totalOrder = orders.Count();
            var orderPending = orders.Count(o => o.Status == (int)StatusOrder.Pending);
            var orderCompleted = orders.Count(o => o.Status == (int)StatusOrder.Completed);
            var orderCancelled = orders.Count(o => o.Status == (int)StatusOrder.Cancelled);
            var orderShipping = orders.Count(o => o.Status == (int)StatusOrder.Shipping);
            var orderConfirmedByUser = orders.Count(o => o.Status == (int)StatusOrder.ConfirmedByUser);
            var orderConfirmedByAdmin = orders.Count(o => o.Status == (int)StatusOrder.ConfirmedByAdmin);

            var year = DateTime.Now.Year;
            var allStactics = await _context.Statisticals.Where(s => s.Time.Value.Year == year).ToListAsync();
            var statics = new StaticalInYear()
            {
                Year = year,
                January = allStactics.Sum(s => s.Time.Value.Month == 1 ? s.TotalAmount : 0),
                February = allStactics.Sum(s => s.Time.Value.Month == 2 ? s.TotalAmount : 0),
                March = allStactics.Sum(s => s.Time.Value.Month == 3 ? s.TotalAmount : 0),
                April = allStactics.Sum(s => s.Time.Value.Month == 4 ? s.TotalAmount : 0),
                May = allStactics.Sum(s => s.Time.Value.Month == 5 ? s.TotalAmount : 0),
                June = allStactics.Sum(s => s.Time.Value.Month == 6 ? s.TotalAmount : 0),
                July = allStactics.Sum(s => s.Time.Value.Month == 7 ? s.TotalAmount : 0),
                August = allStactics.Sum(s => s.Time.Value.Month == 8 ? s.TotalAmount : 0),
                September = allStactics.Sum(s => s.Time.Value.Month == 9 ? s.TotalAmount : 0),
                October = allStactics.Sum(s => s.Time.Value.Month == 10 ? s.TotalAmount : 0),
                November = allStactics.Sum(s => s.Time.Value.Month == 11 ? s.TotalAmount : 0),
                December = allStactics.Sum(s => s.Time.Value.Month == 12 ? s.TotalAmount : 0),
            };


            var model = new IndexAdminModel()
            {
                StaticalInYear = statics,
                Orders = orders,
                TotalRevenue = totalRevenue,
                TotalOrder = totalOrder,
                OrderPending = orderPending,
                OrderCompleted = orderCompleted,
                OrderCancelled = orderCancelled,
                OrderShipping = orderShipping,
                OrderConfirmedByUser = orderConfirmedByUser,
                OrderConfirmedByAdmin = orderConfirmedByAdmin,
                BankingMethod = orders.Count(o => o.MethodPay == (int)MethodPayment.Banking),
                CashMethod = orders.Count(o => o.MethodPay == (int)MethodPayment.Cash),
            };*/
            /*return View(model);*/
            ViewData["title"] = "Admin";
            return View();
        }
    }
}
