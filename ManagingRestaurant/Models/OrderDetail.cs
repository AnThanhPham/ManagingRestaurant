using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Models
{
    public class OrderDetail
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public int? Quantity { get; set; }

        [Display(Name = "Price")]
        public decimal? Price { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
