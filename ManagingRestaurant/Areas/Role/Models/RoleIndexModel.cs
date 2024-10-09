namespace ManagingRestaurant.Areas.Role.Models;

public class RoleIndexModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalRoles { get; set; }
    public X.PagedList.IPagedList<RoleModel> roles { get; set; }
}