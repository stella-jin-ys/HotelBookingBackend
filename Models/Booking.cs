using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
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
        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Navigation properties
        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }

        [ForeignKey("RoomID")]
        public Room? Room { get; set; }
        public decimal TotalPrice => Room != null
                   ? (CheckOutDate - CheckInDate).Days * Room.PricePerNight
                   : 0;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}