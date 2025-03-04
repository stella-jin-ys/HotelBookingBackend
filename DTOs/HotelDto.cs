public class HotelDto
{
    public int HotelID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public List<RoomDto> Rooms { get; set; } = new();
}