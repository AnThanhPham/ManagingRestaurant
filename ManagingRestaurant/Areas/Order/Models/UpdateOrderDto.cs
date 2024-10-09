using ManagingRestaurant.Areas.Payment.Models;
using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Areas.Order.Models;

public class UpdateOrderDto
{
    
    public Guid Id { get; set; }
    
    [Required]
    public string Code { get; set; }
    
    [Required]
    public string CustomerName { get; set; }
    
    [Required]
    public string AppUserIdFK { get; set; }
    
    [Required]
    public string CustomerEmail { get; set; }
    
    [Required]
    public string? PhoneNumber { get; set; }
    
    [Required]
    public string Address { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    [Required]
    public MethodPayment MethodPay { get; set; }
    public bool IsConfirmByUser { get; set; } = false;
    public bool IsConfirmByShop { get; set; } = false;
    [Required]
    public StatusOrder Status { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public string? CreatedBy { get; set; }
    
}