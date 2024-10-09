namespace ManagingRestaurant.Models.Cart;

public class ShoppingCartItem
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? ProductCode { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ShoppingCart
{
    public ShoppingCart()
    {
        this.Items = new List<ShoppingCartItem>();
    }
    public List<ShoppingCartItem> Items { get; set; }
    public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
    public int TotalQuantity => Items.Sum(x => x.Quantity);
}