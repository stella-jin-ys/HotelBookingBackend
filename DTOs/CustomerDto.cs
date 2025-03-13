public class CustomerDto
{
    public int CustomerID { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public List<BookingDto>? Bookings { get; set; }
}