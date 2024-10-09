using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Models
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Reservation_Time { set; get; }

        public string? Status { get; set; }

        [Required]
        public string? AppUserId { get; set; }

        public virtual AppUser? AppUser { get; set; }

        public Guid? TableId { get; set; }

        public virtual Table? Table { get; set; }
    }
}
