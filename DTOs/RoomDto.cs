public class RoomDto
{
    public int RoomID { get; set; }
    public int HotelID { get; set; }
    public string RoomNumber { get; set; }
    public string RoomType { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool Available { get; set; }
    public string Description { get; set; }
}