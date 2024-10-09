using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class Product : Common
    {
        public Product()
        {
            Comments = new HashSet<Comment>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        public string? ProductCode { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Image")]
        public string? Image {  get; set; }

        [Column(TypeName = "ntext")]
        public string ShortDesc { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public Guid? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductImage>? ProductImages { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
