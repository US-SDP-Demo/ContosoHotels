using Microsoft.SemanticKernel;
using Microsoft.EntityFrameworkCore;
using ContosoHotels.Data;
using ContosoHotels.Models;
using ContosoHotels.ViewModels;
using System.ComponentModel;
using System.Text.Json;

namespace ContosoHotels.Agents;

public class HousekeepingTools
{
    private readonly ContosoHotelsContext _context;

    public HousekeepingTools(ContosoHotelsContext context)
    {
        _context = context;
    }

    [KernelFunction, Description("Get the all active housekeeping requests for a guest")]
    public async Task<IEnumerable<HousekeepingRequest?>> GetHousekeepingRequestsForGuest(
        [Description("The ID of the guest to retrieve the active housekeeping requests for")]
        int guestId)
    {
        return await _context.HousekeepingRequests
            .Include(hk => hk.Booking)
                .ThenInclude(b => b.Customer)
            .Include(hk => hk.Booking)
                .ThenInclude(b => b.Room)
            .Where(hk => hk.Booking.CustomerId == guestId)
            .Where(hk => hk.Status == HousekeepingRequestStatus.Requested || hk.Status == HousekeepingRequestStatus.InProgress)
            .OrderByDescending(hk => hk.RequestDate)
            .ToListAsync();
    }

}
