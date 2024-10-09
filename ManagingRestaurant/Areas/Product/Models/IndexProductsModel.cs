namespace ManagingRestaurant.Areas.Product.Models;

public class IndexProductsModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalProducts { get; set; }
    public X.PagedList.IPagedList<ManagingRestaurant.Models.Product> products { get; set; }
}