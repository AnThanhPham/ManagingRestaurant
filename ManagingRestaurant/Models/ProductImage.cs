using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string? Image { get; set; }

        public bool IsDefault { get; set; }

        public virtual Product? Product { get; set; }
    }
}
