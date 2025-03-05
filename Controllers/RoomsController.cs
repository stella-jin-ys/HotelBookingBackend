using HotelBookingSystem.Models;
using HotelBookingDb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace HotelBookingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly HotelBookingDbContext _context;

        public RoomsController(HotelBookingDbContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms(bool onlyAvailable = false)
        {
            var query = _context.Rooms.AsQueryable();
            if (onlyAvailable)
            {
                query = query.Where(r => r.Available);
            }
            var rooms = await query.ToListAsync();

            return Ok(rooms.Select(r => new RoomDto
            {
                RoomID = r.RoomID,
                RoomNumber = r.RoomNumber,
                PricePerNight = r.PricePerNight,
                Available = r.Available
            }));
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }
            var roomDto = new RoomDto
            {
                RoomID = room.RoomID,
                HotelID = room.HotelID,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Capacity = room.Capacity,
                PricePerNight = room.PricePerNight,
                Description = room.Description
            };
            return Ok(roomDto);
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, RoomDto roomDto)
        {
            if (id != roomDto.RoomID)
            {
                return BadRequest();
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.RoomNumber = roomDto.RoomNumber;
            room.RoomType = roomDto.RoomType;
            room.Capacity = roomDto.Capacity;
            room.PricePerNight = roomDto.PricePerNight;
            room.Description = roomDto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST: api/Rooms
        [HttpPost]
        public async Task<ActionResult<RoomDto>> PostRoom(RoomDto roomDto)
        {
            var room = new Room
            {
                HotelID = roomDto.HotelID,
                RoomNumber = roomDto.RoomNumber,
                RoomType = roomDto.RoomType,
                Capacity = roomDto.Capacity,
                PricePerNight = roomDto.PricePerNight,
                Description = roomDto.Description,
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var createdRoomDto = new RoomDto
            {
                RoomID = room.RoomID,
                HotelID = room.HotelID,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Capacity = room.Capacity,
                PricePerNight = room.PricePerNight,
                Description = room.Description
            };

            return CreatedAtAction(nameof(GetRoom), new { id = room.RoomID }, createdRoomDto);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomID == id);
        }
    }
}
