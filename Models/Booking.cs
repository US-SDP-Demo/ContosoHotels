using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ContosoHotels.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        CheckedOut,
        Cancelled
    }

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Computed property for view compatibility
        [NotMapped]
        public decimal TotalCost => TotalAmount;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [StringLength(1000)]
        public string? SpecialRequests { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public DateTime? CancellationDate { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId"), JsonIgnore]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("RoomId"), JsonIgnore]
        public virtual Room? Room { get; set; }
    }
}
