
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models;

public class Contact
{
    [Key]
    public Guid Id { get; set; }
    
    [Column(TypeName = "nvarchar(200)")]
    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
    
    [Column(TypeName = "ntext")]
    [Display(Name = "Message")]
    [Required]
    public string Message { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? CreatedAt { get; set; }
    
    public string? CreatedBy { get; set; }
    
    [DefaultValue(false)]
    public bool IsRead { get; set; }
    
}