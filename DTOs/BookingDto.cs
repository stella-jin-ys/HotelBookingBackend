using HotelBookingSystem.Models;

public class BookingDto
{
    public int BookingID { get; set; }
    public int CustomerID { get; set; }
    public int RoomID { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal TotalPrice { get; set; }
}