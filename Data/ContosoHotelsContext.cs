using Microsoft.EntityFrameworkCore;
using ContosoHotels.Models;
using System;

namespace ContosoHotels.Data
{
    public class ContosoHotelsContext : DbContext
    {
        public ContosoHotelsContext(DbContextOptions<ContosoHotelsContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<RoomService> RoomServices { get; set; }
        public DbSet<HousekeepingRequest> HousekeepingRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
            });

            // Configure Room entity
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasIndex(e => e.RoomNumber).IsUnique();
                entity.Property(e => e.RoomNumber).IsRequired();
                entity.Property(e => e.RoomType).IsRequired();
                entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");
            });

            // Configure Booking entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Check-in date must be before check-out date
                entity.ToTable(t => t.HasCheckConstraint("CK_Booking_CheckOutAfterCheckIn", 
                    "[CheckOutDate] > [CheckInDate]"));
            });

            // Configure RoomService entity
            modelBuilder.Entity<RoomService>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ItemName).IsRequired();
                
                entity.HasOne(d => d.Booking)
                    .WithMany()
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure HousekeepingRequest entity
            modelBuilder.Entity<HousekeepingRequest>(entity =>
            {
                entity.HasOne(d => d.Booking)
                    .WithMany()
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data will be added in a separate seeding service
        }
    }
}
