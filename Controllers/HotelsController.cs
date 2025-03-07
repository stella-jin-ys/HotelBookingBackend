using HotelBookingSystem.Models;
using HotelBookingDb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HotelBookingDbContext _context;

        public HotelsController(HotelBookingDbContext context)
        {
            _context = context;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels(
            [FromQuery] string? city,
            [FromQuery] DateTime? checkInDate,
            [FromQuery] DateTime? checkOutDate,
            [FromQuery] int? guest
        )
        {
            var query = _context.Hotels.Include(h => h.Rooms).AsQueryable();
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(h => h.City.ToLower().Contains(city.ToLower()));
            }
            if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                query = query.Where(h => h.Rooms.Any(r => r.Available && r.Capacity >= guest && !_context.Bookings.Any(b => b.CheckInDate <= checkOutDate && b.CheckOutDate >= checkInDate)));
            }
            var hotels = await query.Select(h => new HotelDto
            {
                HotelID = h.HotelID,
                Name = h.Name,
                Address = h.Address,
                City = h.City,
                Country = h.Country,
                Rooms = h.Rooms.Where(r => r.Available && r.Capacity >= guest && !_context.Bookings.Any(b => b.RoomID == r.RoomID && b.CheckOutDate >= DateTime.UtcNow)).Select(r => new RoomDto
                {
                    RoomID = r.RoomID,
                    HotelID = r.HotelID,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    Capacity = r.Capacity,
                    PricePerNight = r.PricePerNight,
                    Description = r.Description,
                    Available = r.Available
                }).ToList()
            }).ToListAsync();

            return Ok(hotels);
        }


        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _context.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.HotelID == id);

            if (hotel == null)
            {
                return NotFound();
            }
            var hotelDto = new HotelDto
            {
                HotelID = hotel.HotelID,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                Rooms = hotel.Rooms.Select(r => new RoomDto
                {
                    RoomID = r.RoomID,
                    HotelID = r.HotelID,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    Capacity = r.Capacity,
                    PricePerNight = r.PricePerNight,
                    Description = r.Description,
                    Available = r.Available
                }).ToList()
            };
            return Ok(hotelDto);
        }

        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if (id != hotelDto.HotelID)
            {
                return BadRequest("Hotel ID mismatch");
            }

            var hotel = await _context.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.HotelID == id);
            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Name = hotelDto.Name;
            hotel.Address = hotelDto.Address;
            hotel.City = hotelDto.City;
            hotel.Country = hotelDto.Country;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        [HttpPost]
        public async Task<ActionResult<HotelDto>> PostHotel(HotelDto hotelDto)
        {
            var hotel = new Hotel()
            {
                Name = hotelDto.Name,
                Address = hotelDto.Address,
                City = hotelDto.City,
                Country = hotelDto.Country,
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            var createdHotelDto = new HotelDto
            {
                HotelID = hotel.HotelID,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country
            };

            return CreatedAtAction(nameof(GetHotel), new { id = hotel.HotelID }, createdHotelDto);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelID == id);
        }
    }
}
