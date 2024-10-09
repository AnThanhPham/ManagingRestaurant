using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
