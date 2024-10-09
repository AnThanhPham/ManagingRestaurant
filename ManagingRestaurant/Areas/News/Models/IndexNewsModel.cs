namespace ManagingRestaurant.Areas.News.Models;

public class IndexNewsModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalNews { get; set; }
    public X.PagedList.IPagedList<ManagingRestaurant.Models.New> news { get; set; }
}