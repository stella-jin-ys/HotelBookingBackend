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
                CheckInDate = b.CheckInDate.Date,
                CheckOutDate = b.CheckOutDate.Date,
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
                CheckInDate = booking.CheckInDate.Date,
                CheckOutDate = booking.CheckOutDate.Date,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status
            };
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> PostBooking(BookingDto bookingDto)
        {
            var room = await _context.Rooms.FindAsync(bookingDto.RoomID);
            if (room == null)
            {
                return BadRequest("Room not found.");
            }

            bool isRoomAvailable = !_context.Bookings.Any(b => b.RoomID == bookingDto.RoomID && ((bookingDto.CheckInDate >= b.CheckInDate && bookingDto.CheckInDate < b.CheckOutDate) || (bookingDto.CheckOutDate > b.CheckInDate && bookingDto.CheckOutDate <= b.CheckOutDate) || (bookingDto.CheckInDate <= b.CheckInDate && bookingDto.CheckOutDate >= b.CheckOutDate))
            );
            if (!isRoomAvailable)
            {
                return BadRequest("The room is not available fro the seleted dates.");
            }

            var newBooking = new Booking
            {
                CustomerID = bookingDto.CustomerID,
                RoomID = bookingDto.RoomID,
                CheckInDate = bookingDto.CheckInDate.Date,
                CheckOutDate = bookingDto.CheckOutDate.Date,
                Status = bookingDto.Status,
                TotalPrice = (bookingDto.CheckOutDate - bookingDto.CheckInDate).Days * room.PricePerNight
            };

            _context.Bookings.Add(newBooking);
            room.Available = false;
            _context.Rooms.Update(room);

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
                return BadRequest("Booking ID mismatch.");
            }

            var bookingToUpdate = await _context.Bookings.FindAsync(id);
            if (bookingToUpdate == null)
            {
                return NotFound("Booking not found.");
            }

            bool isRoomAvailable = !_context.Bookings.Any(b => b.RoomID == bookingDto.
            RoomID && b.BookingID != id && ((bookingDto.CheckInDate >= b.CheckInDate && bookingDto.CheckInDate < b.CheckOutDate) || (bookingDto.CheckOutDate > b.CheckInDate && bookingDto.CheckOutDate <= b.CheckOutDate) || (bookingDto.CheckInDate <= b.CheckInDate && bookingDto.CheckOutDate >= b.CheckOutDate)));

            if (!isRoomAvailable)
            {
                return BadRequest("The room is not available for the updated dates.");
            }
            bookingToUpdate.CustomerID = bookingDto.CustomerID;
            bookingToUpdate.RoomID = bookingDto.RoomID;
            bookingToUpdate.CheckInDate = bookingDto.CheckInDate.Date;
            bookingToUpdate.CheckOutDate = bookingDto.CheckOutDate.Date;
            bookingToUpdate.Status = bookingDto.Status;
            bookingToUpdate.TotalPrice = (bookingDto.CheckOutDate - bookingDto.CheckInDate).Days * (_context.Rooms.FirstOrDefault(r => r.RoomID == bookingDto.RoomID)?.PricePerNight ?? 0);

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
            var room = await _context.Rooms.FindAsync(booking.RoomID);
            if (room != null)
            {
                bool hasOtherBookings = _context.Bookings.Any(b => b.RoomID == room.RoomID && b.BookingID != id);
                if (!hasOtherBookings)
                {
                    room.Available = true;
                    _context.Rooms.Update(room);
                }
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
