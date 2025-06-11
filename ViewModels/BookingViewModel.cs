using ContosoHotels.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoHotels.ViewModels
{
    public class BookingViewModel
    {
        public int RoomId { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 10)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        [StringLength(1000)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        // Room information for display
        public Room? Room { get; set; }
        
        // Customer information for the form
        public Customer Customer { get; set; } = new Customer();
        
        // Computed properties
        public decimal TotalAmount => Room != null ? Room.PricePerNight * (decimal)(CheckOutDate - CheckInDate).TotalDays : 0;
        public decimal TotalCost => TotalAmount;
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
    }
}
