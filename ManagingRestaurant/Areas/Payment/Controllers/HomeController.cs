using System;
using System.Text;
using ManagingRestaurant.Areas.Payment.Models;
using ManagingRestaurant.Models.Cart;
using ManagingRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Cart.Model;
using ManagingRestaurant.Areas.Payment.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Models.Cart;
using Newtonsoft.Json;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.Payment.Controllers;

[Area("Payment")]
[Authorize]
public class HomeController : Controller
{
    private readonly RestaurantContext _context;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;
    public HomeController(RestaurantContext context, UserManager<AppUser> userManager, IWebHostEnvironment hostingEnvironment, IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _hostingEnvironment = hostingEnvironment;
        _emailSender = emailSender;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ShoppingCart cart;
        // Lấy dữ liệu từ Session dưới dạng mảng byte
        byte[] cartData = HttpContext.Session.Get("Cart");
        // Kiểm tra nếu dữ liệu có tồn tại
        if (cartData != null)
        {
            // Chuyển đổi mảng byte thành chuỗi JSON
            string cartJson = Encoding.UTF8.GetString(cartData);

            // Deserialize JSON thành đối tượng ShoppingCart
            cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
        }
        else
        {
            // Khởi tạo đối tượng ShoppingCart
            cart = new ShoppingCart();
            var model = new IndexPayModel()
            {
                Cart = cart
            };
            return View(model);
        }
        var user = await _userManager.GetUserAsync(User);
        var modelIndex = new IndexPayModel()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Cart = cart
        };
        return View(modelIndex);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePayment(IndexPayModel model)
    {
        if (ModelState.IsValid)
        {
            ShoppingCart cart;
            // Lấy dữ liệu từ Session dưới dạng mảng byte
            byte[] cartData = HttpContext.Session.Get("Cart");
            // Kiểm tra nếu dữ liệu có tồn tại
            if (cartData != null)
            {
                // Chuyển đổi mảng byte thành chuỗi JSON
                string cartJson = Encoding.UTF8.GetString(cartData);

                // Deserialize JSON thành đối tượng ShoppingCart
                cart = JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
            }
            else
            {
                // Khởi tạo đối tượng ShoppingCart
                cart = new ShoppingCart();
                return View(new SuccessOrder() { Success = false });
            }

            // Create Order
            var order = new ManagingRestaurant.Models.Order()
            {
                Code = "Order-" + Guid.NewGuid().ToString(),
                CustomerName = model.UserName,
                CustomerEmail = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                AppUserId = model.UserId,
                TotalAmount = cart.TotalPrice,
                Quantity = cart.TotalQuantity,
                MethodPay = (int)model.PaymentMethod,
                IsConfirmByUser = false,
                IsConfirmByShop = false,
                Status = (int)StatusOrder.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = model.UserName,
                UpdatedBy = model.UserName,
            };
            cart.Items.ForEach(p =>
            {
                order.OrderDetails.Add(new OrderDetail()
                {
                    ProductId = Guid.Parse(p.Id.ToString()),
                    Quantity = p.Quantity,
                    Price = p.Price,
                });
            });

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            var html = GetHtmlSendToCustomer(model, order, cart);
            await _emailSender.SendEmailAsync(model.Email, "Order Information", html);
            return View(new SuccessOrder() { Success = true });
        }
        return View(new SuccessOrder() { Success = false });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Confirm([FromQuery] string code, [FromQuery] string email)
    {
        var succcess = new SuccessOrder()
        {
            Success = false
        };
        var order = await _context.Orders.FirstOrDefaultAsync(p => p.Code == code);
        if (order != null && order.CustomerEmail == email)
        {
            succcess.Success = true;
            order.IsConfirmByUser = true;
            order.Status = (int)StatusOrder.ConfirmedByUser;
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
        }
        return View(succcess);
    }

    public string GetHtmlSendToCustomer(IndexPayModel model, ManagingRestaurant.Models.Order order, ShoppingCart cart)
    {
        string body = "";
        foreach (var item in cart.Items)
        {
            body += "<tr>";
            body += "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;word-wrap:break-word\">" + item.Title + "</td>";
            body += "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif\">" + item.Quantity.ToString() + "</td>";
            body += "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif\">" + "<span>" + item.TotalPrice.ToString() + "&nbsp;<span>$</span></span>" + "</td>";
            body += "</tr>";
        }

        var methodPayment = (MethodPayment)order.MethodPay;
        string confirmLink = $"https://localhost:7063/Payment/Home/Confirm/?code={order.Code}&email={model.Email}";

        string html = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.WebRootPath, "Assets", "Payment", "SendToCustomer.html"));
        html = html.Replace("{{OrderBody}}", body);
        html = html.Replace("{{OrderConfirm}}", confirmLink);
        html = html.Replace("{{UserName}}", model.UserName);
        html = html.Replace("{{UserEmail}}", model.Email);
        html = html.Replace("{{UserPhoneNumber}}", model.PhoneNumber);
        html = html.Replace("{{UserAddress}}", model.Address);
        html = html.Replace("{{OrderCode}}", order.Code);
        html = html.Replace("{{OrderTotalProductPrice}}", order.TotalAmount.ToString("N0"));
        html = html.Replace("{{OrderPaymentMethod}}", methodPayment.ToString());
        html = html.Replace("{{OrderTotalPrice}}", (order.TotalAmount + 2).ToString());
        html = html.Replace("{{OrderCreatedAt}}", DateTime.Parse(order.CreatedAt.ToString()).ToString("dd/MM/yyyy"));
        return html;
    }
}