public class CustomerDto
{
    public int CustomerID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<BookingDto>? Bookings { get; set; }
}