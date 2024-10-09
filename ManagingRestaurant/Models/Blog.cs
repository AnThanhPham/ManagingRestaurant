using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingRestaurant.Models
{
    public class Blog : Common
    {
        public Blog()
        {
        }

        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Image")]
        [Column(TypeName = "ntext")]
        public string? Image { get; set; }

        [Column(TypeName = "ntext")]
        public string ShortDesc { get; set; }


        [Column(TypeName = "ntext")]
        public string Desciption { get; set; }

        public bool IsActive { get; set; }

        public string? AppUserId { get; set; }

        public virtual AppUser? AppUser { get; set; }
    }
}
