using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Models;

public class BodyContactModel
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    [Required]
    public string? Message { get; set; }
}