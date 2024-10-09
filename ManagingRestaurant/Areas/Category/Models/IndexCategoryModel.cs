namespace ManagingRestaurant.Areas.Category.Models;

public class IndexCategoryModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalCategories { get; set; }
    public X.PagedList.IPagedList<ManagingRestaurant.Models.Category> categories { get; set; }
}