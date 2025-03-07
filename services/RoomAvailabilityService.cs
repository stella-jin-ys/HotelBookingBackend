using HotelBookingDb.Data;
using Microsoft.EntityFrameworkCore;

public class RoomAvailabilityService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    public RoomAvailabilityService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();
                var today = DateTime.Today;

                var roomsToUpdate = await context.Rooms.Where(r => !r.Available && !context.Bookings.Any(b => b.RoomID == r.RoomID && b.CheckOutDate > today)).ToListAsync();
                foreach (var room in roomsToUpdate)
                {
                    room.Available = true;
                }
                if (roomsToUpdate.Any())
                {
                    await context.SaveChangesAsync();
                }
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}