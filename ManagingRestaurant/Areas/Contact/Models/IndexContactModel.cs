namespace ManagingRestaurant.Models;

public class IndexContactModel
{
    public int ITEM_PER_PAGE { get; set; }
    public int totalContacts { get; set; }
    public X.PagedList.IPagedList<Contact> contacts { get; set; }
}