using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHotels.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerNight { get; set; }

        public int MaxOccupancy { get; set; }

        [StringLength(500)]
        public string? Amenities { get; set; }

        public bool HasWifi { get; set; } = true;

        public bool HasAirConditioning { get; set; } = true;

        public bool HasTv { get; set; } = true;

        public bool HasMinibar { get; set; } = false;

        public bool HasBalcony { get; set; } = false;

        public bool HasOceanView { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
