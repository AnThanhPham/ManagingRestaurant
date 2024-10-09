namespace ManagingRestaurant.Areas.Blogs.Models;

public class IndexBlogsModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalBlogs { get; set; }
    public X.PagedList.IPagedList<ManagingRestaurant.Models.Blog> blogs { get; set; }
}