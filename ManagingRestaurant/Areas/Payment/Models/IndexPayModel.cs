using System.ComponentModel.DataAnnotations;
using ManagingRestaurant.Models.Cart;

namespace ManagingRestaurant.Areas.Payment.Models;

public class IndexPayModel
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? UserId { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Address { get; set; }

    [Required]
    public MethodPayment PaymentMethod { get; set; }
    public ShoppingCart? Cart { get; set; }
}

public enum MethodPayment
{
    Banking,
    Cash,
}

public enum StatusOrder
{
    Pending,
    ConfirmedByUser,
    ConfirmedByAdmin,
    Shipping,
    Completed,
    Cancelled
}

