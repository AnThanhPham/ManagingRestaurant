using ManagingRestaurant.Areas.Payment.Models;

namespace ManagingRestaurant.Areas.Order.Models;

public class IndeOrdersModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalOrders { get; set; }
    
    public StatusOrder StatusOrder { get; set; }
    public X.PagedList.IPagedList<ManagingRestaurant.Models.Order> orders { get; set; }
}