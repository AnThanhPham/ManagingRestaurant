using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Orders = new HashSet<Order>();
        }

        public int Status;

        [Column(TypeName = "ntext")]
        [DefaultValue("https://www.pngkey.com/png/full/72-729716_user-avatar-png-graphic-free-download-icon.png")]
        public string? Image { get; set; }

        [MaxLength(100)]
        public string? FullName { set; get; }

        [MaxLength(255)]
        public string? Address { set; get; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { set; get; }

        [DataType(DataType.Date)]
        public DateTime? Created_at { set; get; }

        [MaxLength(255)]
        public string? Created_by { set; get; }

        [DataType(DataType.Date)]
        public DateTime? Update_at { set; get; }

        [MaxLength(255)]
        public string? Update_by { set; get; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
