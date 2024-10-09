namespace ManagingRestaurant.Areas.User.Models;

public class WishListAddModel
{
    public string? UserId { get; set; }
    public string? ProductId { get; set; }
    
    public bool IsAdd { get; set; }
}