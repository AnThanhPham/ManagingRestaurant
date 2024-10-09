using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagingRestaurant.Areas.Comment.Models;
using ManagingRestaurant.Models;
using ManagingRestaurant.Data;

namespace ManagingRestaurant.Areas.Comment.Controllers
{
    [Area("Comment")]
    [Authorize]
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
        

        // POST: Comments/Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]BodyCommentModel data)
        {
            if (data != null && ModelState.IsValid)
            {
                if(data.Content == null || data.Rating == 0)
                {
                    return Json(new { success = false, message = "Invalid data request!" });
                }

                if (data.AppUserId == null)
                {
                    return Json(new { success = false, message = "Require login" });
                }
                
                var checkUser = await _context.Comments.FirstOrDefaultAsync(x => x.ProductId == data.ProductId && x.AppUserId == data.AppUserId.ToString());
                if (checkUser != null)
                {
                    return Json(new { success = false, message = "User has already commented on this product!" });
                }
                var comment = new ManagingRestaurant.Models.Comment();
                comment.Id = Guid.NewGuid();
                comment.ProductId = data.ProductId;
                comment.AppUserId = data.AppUserId.ToString();
                comment.Content = data.Content;
                comment.Rating = data.Rating;
                comment.CreatedAt = DateTime.Now;
                comment.CreatedBy = User.Identity.Name;
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Comment has been created!" });
            }
            return Json(new { success = false, message = "Invalid data request!" });
        }
        
    }
}
