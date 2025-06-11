using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoHotels.ViewModels
{
    public class RoomSearchViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);

        [Range(1, 10)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 2;        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "Room Type")]
        public string? RoomType { get; set; }

        [Display(Name = "Maximum Price per Night")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Ocean View")]
        public bool OceanView { get; set; }

        [Display(Name = "Balcony")]
        public bool Balcony { get; set; }

        [Display(Name = "Minibar")]
        public bool Minibar { get; set; }
    }
}
