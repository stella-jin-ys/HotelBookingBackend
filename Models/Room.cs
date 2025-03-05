using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingSystem.Models
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }

        [Required]
        public int HotelID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string RoomType { get; set; }

        public int Capacity { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal PricePerNight { get; set; }
        public bool Available { get; set; } = true;

        public string Description { get; set; }

        // Navigation properties
        [ForeignKey("HotelID")]
        public Hotel? Hotel { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}