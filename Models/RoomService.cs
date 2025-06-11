using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHotels.Models
{
    public enum RoomServiceType
    {
        Food,
        Beverage,
        Amenity,
        Other
    }

    public enum RoomServiceStatus
    {
        Requested,
        InProgress,
        Delivered,
        Cancelled
    }

    public class RoomService
    {
        [Key]
        public int RoomServiceId { get; set; }

        [Required]
        public int BookingId { get; set; }        [Required]
        [StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ItemDescription { get; set; }

        [Required]
        public RoomServiceType ServiceType { get; set; }

        [Required]
        public RoomServiceStatus Status { get; set; } = RoomServiceStatus.Requested;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        // Navigation property
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
    }
}
