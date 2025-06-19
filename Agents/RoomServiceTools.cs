using Microsoft.SemanticKernel;
using Microsoft.EntityFrameworkCore;
using ContosoHotels.Data;
using ContosoHotels.Models;
using ContosoHotels.ViewModels;
using System.ComponentModel;
using System.Text.Json;

namespace ContosoHotels.Agents
{
    public class RoomServiceTools
    {
        private readonly ContosoHotelsContext _context;

        public RoomServiceTools(ContosoHotelsContext context)
        {
            _context = context;
        }

        [KernelFunction, Description("Get the all active room service orders for a guest")]
        public async Task<IEnumerable<RoomService?>> GetRoomServiceOrderForGuest(
            [Description("The ID of the guest to retrieve the active orders for")]
            int guestId)
        {
            return await _context.RoomServices
                .Include(rs => rs.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(rs => rs.Booking)
                    .ThenInclude(b => b.Room)
                .Where(rs => rs.Booking.CustomerId == guestId)
                .Where(rs => rs.Status == RoomServiceStatus.Requested || rs.Status == RoomServiceStatus.InProgress)
                .OrderByDescending(rs => rs.RequestDate)
                .ToListAsync();
        }
    }
}
