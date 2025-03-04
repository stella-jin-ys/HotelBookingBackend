using HotelBookingDb.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly HotelBookingDbContext _context;

        public BookingsController(HotelBookingDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Customer)
                .ToListAsync();

            return bookings.Select(b => new BookingDto
            {
                BookingID = b.BookingID,
                CustomerID = b.CustomerID,
                RoomID = b.RoomID,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalPrice = b.TotalPrice,
                Status = b.Status
            }).ToList();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return new BookingDto
            {
                BookingID = booking.BookingID,
                CustomerID = booking.CustomerID,
                RoomID = booking.RoomID,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = (booking.CheckOutDate - booking.CheckInDate).Days * (booking.Room?.PricePerNight ?? 0),
                Status = booking.Status
            };
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> PostBooking(BookingDto bookingDto)
        {
            // Check if the room is available for the given dates
            bool isRoomAvailable = !_context.Bookings.Any(b =>
                b.RoomID == bookingDto.RoomID &&
                ((bookingDto.CheckInDate >= b.CheckInDate && bookingDto.CheckInDate < b.CheckOutDate) ||
                (bookingDto.CheckOutDate > b.CheckInDate && bookingDto.CheckOutDate <= b.CheckOutDate) ||
                (bookingDto.CheckInDate <= b.CheckInDate && bookingDto.CheckOutDate >= b.CheckOutDate)));

            if (!isRoomAvailable)
            {
                return BadRequest("The room is not available for the selected dates.");
            }

            var newBooking = new Booking
            {
                CustomerID = bookingDto.CustomerID,
                RoomID = bookingDto.RoomID,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate,
                Status = bookingDto.Status
            };

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            bookingDto.BookingID = newBooking.BookingID;
            return CreatedAtAction(nameof(GetBooking), new { id = newBooking.BookingID }, bookingDto);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, BookingDto bookingDto)
        {
            if (id != bookingDto.BookingID)
            {
                return BadRequest();
            }

            // Ensure the room is still available
            bool isRoomAvailable = !_context.Bookings.Any(b =>
                b.RoomID == bookingDto.RoomID &&
                b.BookingID != id &&
                ((bookingDto.CheckInDate >= b.CheckInDate && bookingDto.CheckInDate < b.CheckOutDate) ||
                (bookingDto.CheckOutDate > b.CheckInDate && bookingDto.CheckOutDate <= b.CheckOutDate) ||
                (bookingDto.CheckInDate <= b.CheckInDate && bookingDto.CheckOutDate >= b.CheckOutDate)));

            if (!isRoomAvailable)
            {
                return BadRequest("The room is not available for the updated dates.");
            }

            var bookingToUpdate = await _context.Bookings.FindAsync(id);
            if (bookingToUpdate == null)
            {
                return NotFound();
            }

            // Update properties
            bookingToUpdate.CustomerID = bookingDto.CustomerID;
            bookingToUpdate.RoomID = bookingDto.RoomID;
            bookingToUpdate.CheckInDate = bookingDto.CheckInDate;
            bookingToUpdate.CheckOutDate = bookingDto.CheckOutDate;
            bookingToUpdate.Status = bookingDto.Status;

            _context.Entry(bookingToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }
    }
}
