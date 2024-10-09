using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Cart.Model;
using ManagingRestaurant.Models;
using ManagingRestaurant.Models.Cart;
using Newtonsoft.Json;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.Cart.Controllers;

[Area("Cart")]
[Authorize]
public class HomeController : Controller
{
    private readonly RestaurantContext _context;
    public HomeController(RestaurantContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] CartAddModel data)
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
        }
        
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.ToString() == data.id);
        if (product == null)
        {
            return Json(new { success = false, message = "Product not found!" });
        }
        var item = new ShoppingCartItem()
        {
            Id = product.Id.ToString(),
            Title = product.Name,
            ProductCode = product.ProductCode,
            Price = product.Price,
            Quantity = data.quantity,
            Image = product.Image,
            TotalPrice = product.Price * data.quantity
        };
        var findItem = cart.Items.FirstOrDefault(i => i.Id == item.Id);
        if(findItem != null)
        {
            if(findItem.Quantity + item.Quantity > product.Quantity)
            {
                return Json(new { success = false, message = "Quantity of product in cart must be less than " + product.Quantity.ToString() });
            }
            findItem.Quantity += item.Quantity;
            findItem.TotalPrice = findItem.Quantity * findItem.Price;
        }
        else
        {
            if(item.Quantity > product.Quantity)
            {
                return Json(new { success = false, message = "Quantity of product in cart must be less than " + product.Quantity.ToString() });
            }
            cart.Items.Add(item);
        }
        HttpContext.Session.Set("Cart", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cart)));
        return Json(new { success = true, message = "Add to cart successfully!"});
    }
    
    [HttpPost]
    public async Task<IActionResult> DecreaseToCart([FromBody] CartAddModel data)
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
        }
        
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.ToString() == data.id);
        if (product == null)
        {
            return Json(new { success = false, message = "Product not found!" });
        }
        var findItem = cart.Items.FirstOrDefault(i => i.Id == data.id);
        if(findItem != null)
        {
            if(findItem.Quantity - data.quantity < 0)
            {
                return Json(new { success = false, message = "Quantity of product in cart must be more than 0"});
            }
            findItem.Quantity -= data.quantity;
            findItem.TotalPrice = findItem.Quantity * findItem.Price;
        }
        else
        {
            return Json(new { success = false, message = "Product not found in cart!" });
        }
        HttpContext.Session.Set("Cart", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cart)));
        return Json(new { success = true, message = "Decrease product cart successfully!"});
    }
    
    
    [HttpPost]
    public async Task<IActionResult> DeleteToCart([FromBody] CartAddModel data)
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
        }

        if (data.quantity == -1)
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true, message = "Delete cart successfully!"});
        }
        
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.ToString() == data.id);
        if (product == null)
        {
            return Json(new { success = false, message = "Product not found!" });
        }
        var findItem = cart.Items.FirstOrDefault(i => i.Id == data.id);
        if(findItem != null)
        {
            cart.Items.Remove(findItem);
        }
        else
        {
            return Json(new { success = false, message = "Product not found in cart!" });
        }
        HttpContext.Session.Set("Cart", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cart)));
        return Json(new { success = true, message = "Delete product cart successfully!"});
    }
    
    [HttpGet]
    public IActionResult GetSession()
    {
        byte[] cartData = HttpContext.Session.Get("Cart");
        if (cartData != null)
        {
            string cartJson = Encoding.UTF8.GetString(cartData);
            return Content(cartJson);
        }
        return Ok(new {TotalQuantity = 0, TotalPrice = 0});
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult CartIndex()
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
        }

        return View(cart);
    }
    
}