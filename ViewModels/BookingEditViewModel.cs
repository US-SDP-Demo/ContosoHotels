using System;
using System.ComponentModel.DataAnnotations;
using ContosoHotels.Models;

namespace ContosoHotels.ViewModels
{
    public class BookingEditViewModel
    {
        public int BookingId { get; set; }

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
        public int NumberOfGuests { get; set; }        [StringLength(1000)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }

        // Current values for comparison
        public DateTime CurrentCheckInDate { get; set; }
        public DateTime CurrentCheckOutDate { get; set; }
        public decimal CurrentTotalAmount { get; set; }

        // Related objects for display
        public Room? Room { get; set; }
        public Customer? Customer { get; set; }

        // Computed properties
        public decimal NewTotalAmount => Room != null ? Room.PricePerNight * (decimal)(CheckOutDate - CheckInDate).TotalDays : 0;
        public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
        public bool HasChanges => CheckInDate != CurrentCheckInDate || 
                                 CheckOutDate != CurrentCheckOutDate || 
                                 NewTotalAmount != CurrentTotalAmount;
    }
}
