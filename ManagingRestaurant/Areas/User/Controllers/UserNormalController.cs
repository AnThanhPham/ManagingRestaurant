/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Order.Models;
using ManagingRestaurant.Areas.Payment.Models;
using ManagingRestaurant.Areas.User.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Models.Cart;
using ManagingRestaurant.UpLoad;
using X.PagedList;

namespace ManagingRestaurant.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class UserNormalController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUploadService _uploadService;
        
    public string ImageDefault = "https://www.pngkey.com/png/full/72-729716_user-avatar-png-graphic-free-download-icon.png";

    public UserNormalController(AppDbContext context, UserManager<AppUser> userManager, IUploadService uploadService)
    {
        _context = context;
        _userManager = userManager;
        _uploadService = uploadService;
    }
    [TempData]
    public string StatusMessage { get; set; }
    public const int ITEM_PER_PAGE = 5;
    public const int ITEM_PER_PAGE_WISHLIST = 6;
    
    // GET: User/User/Edit/5
        public async Task<IActionResult> EditRender(Guid? id)
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
        public async Task<IActionResult> EditRender(Guid id, IFormFile? file,
            [Bind("Id,UserName,Email,PhoneNumber,Image,Address,Job,FbLink,IgLink,OtherLink,BirthDay,EmailConfirmed,PhoneNumberConfirmed")] 
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
                            string imageLink = await _uploadService.UploadFile(file);
                            if(!string.IsNullOrEmpty(imageLink))
                            {
                                if (userToUpdate.Image != null && userToUpdate.Image != ImageDefault)
                                {
                                    await _uploadService.DeleteFile(userToUpdate.Image);
                                }
                                userToUpdate.Image = imageLink;
                            }
                        }
                        
                        userToUpdate.UserName = user.UserName;
                        userToUpdate.Email = user.Email;
                        userToUpdate.PhoneNumber = user.PhoneNumber;
                        userToUpdate.Address = user.Address;
                        userToUpdate.Job = user.Job;
                        userToUpdate.FbLink = user.FbLink;
                        userToUpdate.IgLink = user.IgLink;
                        userToUpdate.OtherLink = user.OtherLink;
                        userToUpdate.BirthDay = user.BirthDay;
                        userToUpdate.UpdatedAt = DateTime.Now;
                        userToUpdate.UpdatedBy = User.Identity?.Name;
                        userToUpdate.EmailConfirmed = user.EmailConfirmed;
                        userToUpdate.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                        await _context.SaveChangesAsync();
                        StatusMessage = "Updated info successfully.";
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
                return RedirectToAction(nameof(EditRender), new {id = user.Id});
            }
            return View(user);
        }
        
        [HttpGet]
        public async Task<IActionResult> HistoryOrder(string? id,int? status, int? page = 1 )
        {
            var order = _context.Orders
                .Where(x => x.AppUserIdFK == id).OrderByDescending( n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            if (status != -1 && status != null)
            {
                order = _context.Orders.Where(o => o.Status == status && o.AppUserIdFK == id).OrderByDescending(n => n.CreatedAt).ToPagedList(page ?? 1, ITEM_PER_PAGE);
            }
            var model = new IndeOrdersModel()
            {
                ITEM_PER_PAGE = ITEM_PER_PAGE,
                totalOrders = order.Count(),
                orders = order,
                StatusOrder = (StatusOrder)status.GetValueOrDefault()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> HistoryDetail(string? id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(x => x.Id.ToString() == id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UserDeleteOrder(string? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id.ToString() == id && o.AppUserIdFK == user.Id);
            
            
            if (order == null)
            {
                StatusMessage = "Error: Order not found.";
                return RedirectToAction(nameof(HistoryOrder), new {id = user.Id});
            }
            order.Status = (int)StatusOrder.Cancelled;
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = user.UserName;
            StatusMessage = "Order has been cancelled.";
            //Create notification
            var notification = new ManagingRestaurant.Models.Notification()
            {
                Id = new Guid(),
                Description = $"User {user.UserName} has cancelled an order with code {order.Code}",
                Type = TypeNotification.Warning,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = user.UserName,
                UpdatedBy = user.UserName,
            };
        
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HistoryOrder), new {id = user.Id});
        }
        
        [HttpGet]
        public async Task<IActionResult> GetWishList(int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            var query = from p in _context.Products
                join w in _context.WishLists on p.Id.ToString() equals w.ProductId
                where w.UserId == user.Id
                select p;
            var model = query.ToPagedList(page ?? 1, ITEM_PER_PAGE_WISHLIST);
            return View(model);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddWishList([FromBody] WishListAddModel data)
        {
            if(data.UserId == null || data.ProductId == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "UserId or ProductId is null ",
                });
            }
            //check User
           var user = await _context.Users.FirstOrDefaultAsync( u => u.Id == data.UserId);
           if(user == null)
           {
               return Ok(new
               {
                   success = false,
                   message = "User not found"
               });
           }
           else
           {
               var checkUser = await _userManager.GetUserAsync(User);
                if (checkUser.Id != user.Id)
                {
                     return Ok(new
                     {
                          success = false,
                          message = "You are not this User"
                     });
                }
           }
           //check Product
           var product = await _context.Products.FirstOrDefaultAsync( p => p.Id.ToString() == data.ProductId);
           if(product == null)
           {
               return Ok(new
               {
                   success = false,
                   message = "Product not found",
               });
           }
           //find wishList
           var findWishList = await _context.WishLists.FirstOrDefaultAsync( w => w.UserId == data.UserId && w.ProductId == data.ProductId);
           var wishList = new WishList()
           {
               UserId = data.UserId,
               ProductId = data.ProductId,
           };
           if (data.IsAdd)
           {
               if (findWishList == null)
               {
                   await _context.WishLists.AddAsync(wishList);
               }
               else
               {
                   return Ok(new
                   {
                       success = false,
                       message = "Product already exists in the wish list"

                   });
               }
               
           }
           else
           {
               if (findWishList == null)
               {
                   return Ok(new
                   {
                       success = false,
                       message = "Product is not exists in the wish list"
                   });
               }
               else
               {
                   _context.WishLists.Remove(findWishList);
               }
           }
              await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = data.IsAdd ? "Add wish list successfully" : "Remove wish list successfully"
            });
        }
        
        
        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
}*/