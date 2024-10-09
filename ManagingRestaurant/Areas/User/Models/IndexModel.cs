using ManagingRestaurant.Models;

namespace ManagingRestaurant.Areas.User.Models;

public class IndexModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalUsers { get; set; }
    public X.PagedList.IPagedList<AppUser> users { get; set; }
}