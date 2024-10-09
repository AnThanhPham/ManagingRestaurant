using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class Comment : Common
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        public int Rating { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Required]
        public string? AppUserId { get; set; }

        public virtual AppUser? AppUser { get; set; }
    }
}
