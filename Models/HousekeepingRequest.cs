using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHotels.Models
{
    public enum HousekeepingRequestType
    {
        Towels,
        Cleaning,
        Bedding,
        Toiletries,
        Other
    }

    public enum HousekeepingRequestStatus
    {
        Requested,
        InProgress,
        Completed,
        Cancelled
    }

    public class HousekeepingRequest
    {
        [Key]
        public int HousekeepingRequestId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public HousekeepingRequestType RequestType { get; set; }

        [Required]
        public HousekeepingRequestStatus Status { get; set; } = HousekeepingRequestStatus.Requested;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? CompletionDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
    }
}
