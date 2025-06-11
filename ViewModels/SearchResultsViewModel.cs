using ContosoHotels.Models;
using System.Collections.Generic;

namespace ContosoHotels.ViewModels
{
    public class SearchResultsViewModel
    {
        public RoomSearchViewModel? SearchCriteria { get; set; }
        public List<Room> AvailableRooms { get; set; } = new List<Room>();
        public int TotalResults { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (TotalResults + PageSize - 1) / PageSize;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
