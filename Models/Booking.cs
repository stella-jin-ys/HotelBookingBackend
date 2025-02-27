using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int RoomID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }

        [ForeignKey("RoomID")]
        public Room? Room { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}