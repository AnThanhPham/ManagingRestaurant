using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Areas.Comment.Models;

public class BodyCommentModel
{
    [Required]
    public string? Content { get; set; }
    
    [Required]
    public Guid? ProductId { get; set; }
    
    [Required]
    public Guid? AppUserId { get; set; }
    
    [Required]
    public int Rating { get; set; }
}