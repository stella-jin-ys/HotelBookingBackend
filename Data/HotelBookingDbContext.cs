
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingDb.Data
{
    public class HotelBookingDbContext : DbContext
    {
        public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingID)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public static void SeedData(HotelBookingDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Add seed data only if tables are empty
            if (!context.Hotels.Any())
            {
                var hotels = new List<Hotel>
        {
            new Hotel
            {
                Name = "Grand Hotel",
                Address = "123 Main Street",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "Usa",
                Phone = "212-555-1234",
                Email = "info@grandhotel.com",
                Website = "_blank"
            },
            new Hotel
            {
                Name = "Seaside Resort",
                Address = "456 Ocean Drive",
                City = "Miami",
                State = "FL",
                ZipCode = "33139",
                Country= "Usa",
                Phone = "305-555-6789",
                Email = "contact@seasideresort.com",
                Website = "_blank"
            }
        };
                context.Hotels.AddRange(hotels);
                context.SaveChanges();

                // Add rooms for the hotels
                var rooms = new List<Room>
        {
            new Room
            {
                HotelID = hotels[0].HotelID, // Grand Hotel
                RoomNumber = "101",
                RoomType = "Standard",
                PricePerNight = 150.00m,
                Description = "Comfortable standard room with queen bed",
                Capacity = 2
            },
            new Room
            {
                HotelID = hotels[0].HotelID, // Grand Hotel
                RoomNumber = "201",
                RoomType = "Deluxe",
                PricePerNight = 250.00m,
                Description = "Spacious deluxe room with king bed and city view",
                Capacity = 2
            },
            new Room
            {
                HotelID = hotels[1].HotelID, // Seaside Resort
                RoomNumber = "105",
                RoomType = "Ocean View",
                PricePerNight = 300.00m,
                Description = "Room with spectacular ocean views",
                Capacity = 3
            }
        };
                context.Rooms.AddRange(rooms);
                context.SaveChanges();

                // Add customers
                var customers = new List<Customer>
{
            new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "555-123-4567",
                Address = "789 Pine St"
            },
            new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "555-987-6543",
                Address = "321 Maple Ave"
            }
        };
                context.Customers.AddRange(customers);
                context.SaveChanges();

                var savedCustomers = context.Customers.ToList();
                // Add some bookings
                var bookings = new List<Booking>
            {
            new Booking
            {
                CustomerID = savedCustomers[0].CustomerID,
                RoomID = rooms[0].RoomID,
                CheckInDate = DateTime.Now.AddDays(30),
                CheckOutDate = DateTime.Now.AddDays(35),
                BookingDate = DateTime.Now,
                Status = "Confirmed",
                TotalPrice = rooms[0].PricePerNight * 5 // 5 nights
            },
            new Booking
            {
                CustomerID = savedCustomers[1].CustomerID,
                RoomID = rooms[2].RoomID,
                CheckInDate = DateTime.Now.AddDays(45),
                CheckOutDate = DateTime.Now.AddDays(50),
                BookingDate = DateTime.Now.AddDays(-5),
                Status = "Confirmed",
                TotalPrice = rooms[2].PricePerNight * 5 // 5 nights
            }
        };
                context.Bookings.AddRange(bookings);
                context.SaveChanges();

                // Add payments
                var payments = new List<Payment>
        {
            new Payment
            {
                BookingID = bookings[0].BookingID,
                PaymentDate = DateTime.Now,
                Amount = bookings[0].TotalPrice * 0.3m, // 30% deposit
                PaymentMethod = "Credit Card",
                PaymentStatus = "Completed"
            },
            new Payment
            {
                BookingID = bookings[1].BookingID,
                PaymentDate = DateTime.Now.AddDays(-4),
                Amount = bookings[1].TotalPrice, // Full payment
                PaymentMethod = "PayPal",
                PaymentStatus = "Completed"
            }
        };
                context.Payments.AddRange(payments);
                context.SaveChanges();
            }
        }

    }
}