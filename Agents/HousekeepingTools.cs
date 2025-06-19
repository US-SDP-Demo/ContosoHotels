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

    [KernelFunction]
    [Description("Search and filter housekeeping requests with pagination")]
    public async Task<string> SearchHousekeepingRequestsAsync(
        [Description("Room number to search for")] string? searchRoomNumber = null,
        [Description("Start date for filtering requests (YYYY-MM-DD format)")] string? searchDateFrom = null,
        [Description("End date for filtering requests (YYYY-MM-DD format)")] string? searchDateTo = null,
        [Description("Show only open requests (requested/in-progress)")] bool showOnlyOpenRequests = true,
        [Description("Page number for pagination")] int page = 1,
        [Description("Number of items per page")] int pageSize = 10)
    {
        try
        {
            // Build the query for housekeeping requests
            var query = _context.HousekeepingRequests
                .Include(h => h.Booking!)
                .ThenInclude(b => b.Customer)
                .Include(h => h.Booking!)
                .ThenInclude(b => b.Room)
                .AsQueryable();

            // Apply open requests filter if needed
            if (showOnlyOpenRequests)
            {
                query = query.Where(h => 
                    h.Status == HousekeepingRequestStatus.Requested || 
                    h.Status == HousekeepingRequestStatus.InProgress);
            }

            // Apply search filters
            if (!string.IsNullOrEmpty(searchRoomNumber))
            {
                query = query.Where(h => h.Booking!.Room!.RoomNumber.Contains(searchRoomNumber));
            }

            if (!string.IsNullOrEmpty(searchDateFrom) && DateTime.TryParse(searchDateFrom, out var dateFrom))
            {
                query = query.Where(h => h.RequestDate >= dateFrom);
            }

            if (!string.IsNullOrEmpty(searchDateTo) && DateTime.TryParse(searchDateTo, out var dateTo))
            {
                query = query.Where(h => h.RequestDate <= dateTo);
            }

            // Get total count for pagination
            var totalRequests = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRequests / pageSize);

            // Apply pagination and ordering
            var requests = await query
                .OrderByDescending(h => h.RequestDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new
                {
                    HousekeepingRequestId = h.HousekeepingRequestId,
                    BookingId = h.BookingId,
                    CustomerName = (h.Booking!.Customer!.FirstName ?? "") + " " + (h.Booking.Customer.LastName ?? ""),
                    RoomNumber = h.Booking.Room!.RoomNumber ?? "",
                    RequestType = h.RequestType.ToString(),
                    Status = h.Status.ToString(),
                    RequestDate = h.RequestDate,
                    CompletionDate = h.CompletionDate,
                    Notes = h.Notes ?? string.Empty
                })
                .ToListAsync();

            var result = new
            {
                Requests = requests,
                TotalRequests = totalRequests,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = $"Error searching housekeeping requests: {ex.Message}" });
        }
    }

    [KernelFunction]
    [Description("Get detailed information about a specific housekeeping request")]
    public async Task<string> GetHousekeepingDetailsAsync(
        [Description("The ID of the housekeeping request")] int requestId)
    {
        try
        {
            var request = await _context.HousekeepingRequests
                .Include(h => h.Booking!)
                .ThenInclude(b => b.Customer)
                .Include(h => h.Booking!)
                .ThenInclude(b => b.Room)
                .FirstOrDefaultAsync(m => m.HousekeepingRequestId == requestId);

            if (request == null)
            {
                return JsonSerializer.Serialize(new { error = "Housekeeping request not found" });
            }

            var result = new
            {
                HousekeepingRequestId = request.HousekeepingRequestId,
                BookingId = request.BookingId,
                Customer = new
                {
                    Name = $"{request.Booking!.Customer!.FirstName ?? ""} {request.Booking.Customer.LastName ?? ""}",
                    Email = request.Booking.Customer.Email ?? "",
                    PhoneNumber = request.Booking.Customer.PhoneNumber ?? ""
                },
                Room = new
                {
                    RoomNumber = request.Booking.Room!.RoomNumber ?? "",
                    RoomType = request.Booking.Room.RoomType ?? "",
                    PricePerNight = request.Booking.Room.PricePerNight
                },
                RequestType = request.RequestType.ToString(),
                Status = request.Status.ToString(),
                RequestDate = request.RequestDate,
                CompletionDate = request.CompletionDate,
                Notes = request.Notes ?? string.Empty,
                Booking = new
                {
                    CheckInDate = request.Booking.CheckInDate,
                    CheckOutDate = request.Booking.CheckOutDate,
                    TotalAmount = request.Booking.TotalAmount
                }
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = $"Error retrieving housekeeping details: {ex.Message}" });
        }
    }

    [KernelFunction]
    [Description("Update the status of a housekeeping request")]
    public async Task<string> UpdateHousekeepingStatusAsync(
        [Description("The ID of the housekeeping request")] int requestId,
        [Description("New status: Requested, InProgress, Completed, or Cancelled")] string status)
    {
        try
        {
            var request = await _context.HousekeepingRequests.FindAsync(requestId);
            if (request == null)
            {
                return JsonSerializer.Serialize(new { error = "Housekeeping request not found" });
            }

            if (Enum.TryParse<HousekeepingRequestStatus>(status, out var requestStatus))
            {
                var oldStatus = request.Status.ToString();
                request.Status = requestStatus;
                
                if (requestStatus == HousekeepingRequestStatus.Completed)
                {
                    request.CompletionDate = DateTime.UtcNow;
                }
                
                await _context.SaveChangesAsync();

                var result = new
                {
                    success = true,
                    message = $"Request status updated from {oldStatus} to {status} successfully",
                    requestId = requestId,
                    oldStatus = oldStatus,
                    newStatus = status,
                    completionDate = request.CompletionDate
                };

                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                return JsonSerializer.Serialize(new { error = "Invalid status provided. Valid statuses: Requested, InProgress, Completed, Cancelled" });
            }
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = $"Error updating housekeeping status: {ex.Message}" });
        }
    }

    [KernelFunction]
    [Description("Create a new housekeeping request for a booking")]
    public async Task<string> CreateHousekeepingRequestAsync(
        [Description("The booking ID for the request")] int bookingId,
        [Description("Type of request: Towels, Cleaning, Bedding, Toiletries, or Other")] string requestType,
        [Description("Optional notes for the request")] string? notes = null)
    {
        try
        {
            // Verify booking exists
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return JsonSerializer.Serialize(new { error = "Booking not found" });
            }

            if (Enum.TryParse<HousekeepingRequestType>(requestType, out var parsedRequestType))
            {
                var request = new HousekeepingRequest
                {
                    BookingId = bookingId,
                    RequestType = parsedRequestType,
                    Status = HousekeepingRequestStatus.Requested,
                    RequestDate = DateTime.UtcNow,
                    Notes = notes
                };

                _context.HousekeepingRequests.Add(request);
                await _context.SaveChangesAsync();

                var result = new
                {
                    success = true,
                    message = "Housekeeping request created successfully",
                    requestId = request.HousekeepingRequestId,
                    bookingId = bookingId,
                    requestType = requestType,
                    status = request.Status.ToString(),
                    requestDate = request.RequestDate,
                    customerName = $"{booking.Customer!.FirstName ?? ""} {booking.Customer.LastName ?? ""}",
                    roomNumber = booking.Room!.RoomNumber ?? "",
                    notes = notes ?? string.Empty
                };

                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                return JsonSerializer.Serialize(new { error = "Invalid request type. Valid types: Towels, Cleaning, Bedding, Toiletries, Other" });
            }
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = $"Error creating housekeeping request: {ex.Message}" });
        }
    }

    [KernelFunction]
    [Description("Get statistics about housekeeping requests and completion times")]
    public async Task<string> GetHousekeepingStatsAsync(
        [Description("Number of days to look back for statistics")] int daysBack = 30)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysBack);

            var stats = await _context.HousekeepingRequests
                .Where(h => h.RequestDate >= cutoffDate)
                .GroupBy(h => 1)
                .Select(g => new
                {
                    TotalRequests = g.Count(),
                    CompletedRequests = g.Count(h => h.Status == HousekeepingRequestStatus.Completed),
                    InProgressRequests = g.Count(h => h.Status == HousekeepingRequestStatus.InProgress),
                    RequestedRequests = g.Count(h => h.Status == HousekeepingRequestStatus.Requested),
                    CancelledRequests = g.Count(h => h.Status == HousekeepingRequestStatus.Cancelled),
                    AverageCompletionTimeHours = g.Where(h => h.Status == HousekeepingRequestStatus.Completed && h.CompletionDate.HasValue)
                                                   .Select(h => (h.CompletionDate!.Value - h.RequestDate).TotalHours)
                                                   .DefaultIfEmpty(0)
                                                   .Average()
                })
                .FirstOrDefaultAsync();

            var requestTypeBreakdown = await _context.HousekeepingRequests
                .Where(h => h.RequestDate >= cutoffDate)
                .GroupBy(h => h.RequestType)
                .Select(g => new
                {
                    RequestType = g.Key.ToString(),
                    Count = g.Count(),
                    CompletedCount = g.Count(h => h.Status == HousekeepingRequestStatus.Completed)
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var result = new
            {
                PeriodDays = daysBack,
                OverallStats = stats ?? new
                {
                    TotalRequests = 0,
                    CompletedRequests = 0,
                    InProgressRequests = 0,
                    RequestedRequests = 0,
                    CancelledRequests = 0,
                    AverageCompletionTimeHours = 0.0
                },
                RequestTypeBreakdown = requestTypeBreakdown
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = $"Error retrieving housekeeping statistics: {ex.Message}" });
        }
    }

    [KernelFunction]
    [Description("Get available housekeeping request types")]
    public Task<string> GetRequestTypesAsync()
    {
        try
        {
            var requestTypes = Enum.GetValues<HousekeepingRequestType>()
                .Select(rt => new
                {
                    Value = rt.ToString(),
                    DisplayName = rt.ToString(),
                    Description = rt switch
                    {
                        HousekeepingRequestType.Towels => "Request for fresh towels",
                        HousekeepingRequestType.Cleaning => "Room cleaning service",
                        HousekeepingRequestType.Bedding => "Fresh bed linens and pillows",
                        HousekeepingRequestType.Toiletries => "Bathroom amenities and supplies",
                        HousekeepingRequestType.Other => "Other housekeeping needs",
                        _ => rt.ToString()
                    }
                })
                .ToList();

            var statuses = Enum.GetValues<HousekeepingRequestStatus>()
                .Select(s => new
                {
                    Value = s.ToString(),
                    DisplayName = s.ToString(),
                    Description = s switch
                    {
                        HousekeepingRequestStatus.Requested => "Request has been submitted and is pending",
                        HousekeepingRequestStatus.InProgress => "Housekeeping staff is working on the request",
                        HousekeepingRequestStatus.Completed => "Request has been completed",
                        HousekeepingRequestStatus.Cancelled => "Request has been cancelled",
                        _ => s.ToString()
                    }
                })
                .ToList();

            var result = new
            {
                RequestTypes = requestTypes,
                Statuses = statuses
            };

            return Task.FromResult(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            return Task.FromResult(JsonSerializer.Serialize(new { error = $"Error retrieving request types: {ex.Message}" }));
        }
    }

    /// <summary>
    /// Process natural language housekeeping requests using Semantic Kernel
    /// </summary>
    // public async Task<string> ProcessHousekeepingRequestAsync(string userMessage)
    // {
    //     try
    //     {
    //         var promptTemplate = """
    //             You are a helpful housekeeping assistant for Contoso Hotels. You help guests and staff manage housekeeping requests.
                
    //             Available functions:
    //             - SearchHousekeepingRequests: Search and filter housekeeping requests
    //             - GetHousekeepingDetails: Get details about a specific request
    //             - UpdateHousekeepingStatus: Update request status (Requested, InProgress, Completed, Cancelled)
    //             - CreateHousekeepingRequest: Create new housekeeping requests
    //             - GetHousekeepingStats: Get housekeeping statistics
    //             - GetRequestTypes: Get available request types and statuses
                
    //             User request: {{$userMessage}}
                
    //             Based on the user's request, determine what they want to do and call the appropriate function(s).
    //             If they want to:
    //             - Search for requests: Use SearchHousekeepingRequests
    //             - See request details: Use GetHousekeepingDetails
    //             - Update a request: Use UpdateHousekeepingStatus
    //             - Create a new request: Use CreateHousekeepingRequest
    //             - See statistics: Use GetHousekeepingStats
    //             - Learn about request types: Use GetRequestTypes
                
    //             Respond in a helpful, professional manner appropriate for hotel staff or guests.
    //             """;

    //         var function = _kernel.CreateFunctionFromPrompt(promptTemplate);
    //         var arguments = new KernelArguments { ["userMessage"] = userMessage };

    //         var result = await _kernel.InvokeAsync(function, arguments);
    //         return result.ToString();
    //     }
    //     catch (Exception ex)
    //     {
    //         return JsonSerializer.Serialize(new { error = $"Error processing housekeeping request: {ex.Message}" });
    //     }
    // }
}
