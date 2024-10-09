using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class Order : Common
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public Guid Id { get; set; }

        [Display(Name = "TotalAmount")]
        [Required]
        public decimal TotalAmount { get; set; }

        public int? Status { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "Title")]
        [Required]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "CustomerName")]
        [Required]
        public string CustomerName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "CustomerEmail")]
        [Required]
        public string CustomerEmail { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "Address")]
        [Required]
        public string Address { get; set; }

        public int MethodPay { get; set; }

        [Required]
        public int Quantity { get; set; }

        public bool IsConfirmByUser { get; set; } = false;
        public bool IsConfirmByShop { get; set; } = false;

        [Required]
        public string? AppUserId { get; set; }

        public Guid? TableId { get; set; }

        public virtual Table? Table { get; set; }

        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
