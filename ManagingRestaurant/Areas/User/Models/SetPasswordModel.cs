using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Areas.User.Models;

public class SetPassModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
        
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "ConfirmedPassword")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmedPassword { get; set; }
        
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? FullName { get; set; }
    public string? Image { get; set; }
    public string? Address { get; set; }
}