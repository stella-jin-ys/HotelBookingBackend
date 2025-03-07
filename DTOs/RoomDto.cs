public class RoomDto
{
    public int RoomID { get; set; }
    public int HotelID { get; set; }
    public required string RoomNumber { get; set; }
    public required string RoomType { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool Available { get; set; }
    public required string Description { get; set; }
}