using System.ComponentModel.DataAnnotations;

namespace ManagingRestaurant.Models
{
    public class Table
    {
        public Table()
        {
            Reservations = new HashSet<Reservation>();
            Orders = new HashSet<Order>();
        }

        [Key]
        public Guid Id { get; set; }
        public string Table_Number { get; set; }
        public int Capacity { get; set; }

        public string? Status { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
