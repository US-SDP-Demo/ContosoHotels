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

        [KernelFunction, Description("Get the latest room service order for a guest")]
        public async Task<RoomService?> GetRoomServiceOrderForGuest(
            [Description("The ID of the guest to retrieve the order for")]
            int guestId)
        {
            return await _context.RoomServices
                .Include(r => r.Booking)
                .Include(r => r.Booking.Customer)
                .Include(r => r.Booking.Room)
                .FirstOrDefaultAsync(r => r.Booking.Customer.CustomerId == guestId);
        }

        [KernelFunction]
        [Description("Search room service orders with optional filters")]
        public async Task<string> SearchRoomServiceOrdersAsync(
            [Description("Room number to filter by (optional)")] string? roomNumber = null,
            [Description("Start date for search range (yyyy-MM-dd format, optional)")] string? searchDateFrom = null,
            [Description("End date for search range (yyyy-MM-dd format, optional)")] string? searchDateTo = null,
            [Description("Show only open orders (true/false, default: true)")] bool showOnlyOpenOrders = true,
            [Description("Page number for pagination (default: 1)")] int page = 1,
            [Description("Number of orders per page (default: 10)")] int pageSize = 10)
        {
            try
            {
                // Build the query for room service orders
                var query = _context.RoomServices
                    .Include(r => r.Booking)
                    .Include(r => r.Booking.Customer)
                    .Include(r => r.Booking.Room)
                    .AsQueryable();

                // Apply open orders filter if needed
                if (showOnlyOpenOrders)
                {
                    query = query.Where(r => 
                        r.Status == RoomServiceStatus.Requested || 
                        r.Status == RoomServiceStatus.InProgress);
                }

                // Apply search filters
                if (!string.IsNullOrEmpty(roomNumber))
                {
                    query = query.Where(r => r.Booking.Room.RoomNumber.Contains(roomNumber));
                }

                if (!string.IsNullOrEmpty(searchDateFrom) && DateTime.TryParse(searchDateFrom, out var fromDate))
                {
                    query = query.Where(r => r.RequestDate >= fromDate);
                }

                if (!string.IsNullOrEmpty(searchDateTo) && DateTime.TryParse(searchDateTo, out var toDate))
                {
                    query = query.Where(r => r.RequestDate <= toDate);
                }

                // Get total count for pagination
                var totalOrders = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                // Apply pagination and ordering
                var orders = await query
                    .OrderByDescending(r => r.RequestDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        RoomServiceId = r.RoomServiceId,
                        BookingId = r.BookingId,
                        CustomerName = r.Booking.Customer.FirstName + " " + r.Booking.Customer.LastName,
                        RoomNumber = r.Booking.Room.RoomNumber,
                        ItemName = r.ItemName,
                        ServiceType = r.ServiceType.ToString(),
                        Status = r.Status.ToString(),
                        RequestDate = r.RequestDate,
                        DeliveryDate = r.DeliveryDate,
                        Price = r.Price,
                        SpecialInstructions = r.SpecialInstructions ?? string.Empty,
                        IsOpen = r.Status == RoomServiceStatus.Requested || r.Status == RoomServiceStatus.InProgress
                    })
                    .ToListAsync();

                var result = new
                {
                    Orders = orders,
                    TotalOrders = totalOrders,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    ShowOnlyOpenOrders = showOnlyOpenOrders,
                    SearchCriteria = new
                    {
                        RoomNumber = roomNumber,
                        DateFrom = searchDateFrom,
                        DateTo = searchDateTo
                    }
                };

                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                return $"Error searching room service orders: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Get detailed information about a specific room service order")]
        public async Task<string> GetRoomServiceDetailsAsync(
            [Description("The ID of the room service order")] int roomServiceId)
        {
            try
            {
                var order = await _context.RoomServices
                    .Include(r => r.Booking)
                    .Include(r => r.Booking.Customer)
                    .Include(r => r.Booking.Room)
                    .FirstOrDefaultAsync(m => m.RoomServiceId == roomServiceId);

                if (order == null)
                {
                    return $"Room service order with ID {roomServiceId} not found.";
                }

                var orderDetails = new
                {
                    OrderId = order.RoomServiceId,
                    BookingId = order.BookingId,
                    Customer = new
                    {
                        Name = $"{order.Booking.Customer.FirstName} {order.Booking.Customer.LastName}",
                        Email = order.Booking.Customer.Email,
                        Phone = order.Booking.Customer.PhoneNumber
                    },
                    Room = new
                    {
                        Number = order.Booking.Room.RoomNumber,
                        Type = order.Booking.Room.RoomType
                    },
                    Order = new
                    {
                        ItemName = order.ItemName,
                        Description = order.ItemDescription,
                        ServiceType = order.ServiceType.ToString(),
                        Status = order.Status.ToString(),
                        Price = order.Price,
                        RequestDate = order.RequestDate,
                        DeliveryDate = order.DeliveryDate,
                        SpecialInstructions = order.SpecialInstructions
                    },
                    Booking = new
                    {
                        CheckIn = order.Booking.CheckInDate,
                        CheckOut = order.Booking.CheckOutDate,
                        Guests = order.Booking.NumberOfGuests
                    }
                };

                return JsonSerializer.Serialize(orderDetails, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                return $"Error retrieving room service order details: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Update the status of a room service order")]
        public async Task<string> UpdateRoomServiceStatusAsync(
            [Description("The ID of the room service order to update")] int roomServiceId,
            [Description("New status: Requested, InProgress, Delivered, or Cancelled")] string status)
        {
            try
            {
                var order = await _context.RoomServices.FindAsync(roomServiceId);
                if (order == null)
                {
                    return $"Room service order with ID {roomServiceId} not found.";
                }

                if (!Enum.TryParse<RoomServiceStatus>(status, out var orderStatus))
                {
                    return $"Invalid status '{status}'. Valid statuses are: Requested, InProgress, Delivered, Cancelled.";
                }

                var previousStatus = order.Status.ToString();
                order.Status = orderStatus;
                
                if (orderStatus == RoomServiceStatus.Delivered && !order.DeliveryDate.HasValue)
                {
                    order.DeliveryDate = DateTime.UtcNow;
                }
                
                await _context.SaveChangesAsync();

                return $"Room service order #{roomServiceId} status updated from '{previousStatus}' to '{status}' successfully.";
            }
            catch (Exception ex)
            {
                return $"Error updating room service order status: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Create a new room service order for a booking")]
        public async Task<string> CreateRoomServiceOrderAsync(
            [Description("The booking ID to create the order for")] int bookingId,
            [Description("Name of the item being ordered")] string itemName,
            [Description("Type of service: Food, Beverage, Amenity, or Other")] string serviceType,
            [Description("Price of the item")] decimal price,
            [Description("Special instructions for the order (optional)")] string? specialInstructions = null,
            [Description("Item description (optional)")] string? itemDescription = null)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return $"Booking with ID {bookingId} not found.";
                }

                // Check if booking is active
                if (booking.Status != BookingStatus.CheckedIn && booking.Status != BookingStatus.Confirmed)
                {
                    return $"Room service can only be ordered for active bookings. Current booking status: {booking.Status}";
                }

                if (!Enum.TryParse<RoomServiceType>(serviceType, out var serviceTypeEnum))
                {
                    return $"Invalid service type '{serviceType}'. Valid types are: Food, Beverage, Amenity, Other.";
                }

                var roomService = new RoomService
                {
                    BookingId = bookingId,
                    ItemName = itemName,
                    ItemDescription = itemDescription,
                    ServiceType = serviceTypeEnum,
                    Price = price,
                    Status = RoomServiceStatus.Requested,
                    RequestDate = DateTime.UtcNow,
                    SpecialInstructions = specialInstructions
                };

                _context.RoomServices.Add(roomService);
                await _context.SaveChangesAsync();

                return $"Room service order created successfully. Order ID: {roomService.RoomServiceId}, Item: {itemName}, Price: ${price:F2}";
            }
            catch (Exception ex)
            {
                return $"Error creating room service order: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Get available menu items with pricing for room service")]
        public Task<string> GetMenuItemsAsync(
            [Description("Filter by service type: Food, Beverage, Amenity, Other (optional)")] string? serviceType = null)
        {
            try
            {
                var menuItems = new
                {
                    Food = new[]
                    {
                        new { Name = "Club Sandwich", Price = 18.95m, Description = "Triple-decker with turkey, bacon, lettuce, tomato" },
                        new { Name = "Caesar Salad", Price = 15.50m, Description = "Crisp romaine with parmesan and croutons" },
                        new { Name = "Cheeseburger & Fries", Price = 22.75m, Description = "Angus beef with fries and pickles" },
                        new { Name = "Pasta Carbonara", Price = 19.95m, Description = "Creamy pasta with bacon and parmesan" },
                        new { Name = "Grilled Salmon", Price = 29.95m, Description = "Atlantic salmon with vegetables" },
                        new { Name = "Margherita Pizza", Price = 20.50m, Description = "Fresh mozzarella, basil, tomato sauce" },
                        new { Name = "Steak & Vegetables", Price = 34.95m, Description = "Premium cut with seasonal vegetables" },
                        new { Name = "Chicken Quesadilla", Price = 17.95m, Description = "Grilled chicken with cheese and peppers" }
                    },
                    Beverage = new[]
                    {
                        new { Name = "Fresh Orange Juice", Price = 7.95m, Description = "Freshly squeezed" },
                        new { Name = "Coffee (Premium Blend)", Price = 6.50m, Description = "Freshly brewed" },
                        new { Name = "Tea Selection", Price = 5.95m, Description = "Earl Grey, English Breakfast, Chamomile" },
                        new { Name = "Bottled Water", Price = 4.50m, Description = "Premium spring water" },
                        new { Name = "Soft Drinks", Price = 5.25m, Description = "Coca-Cola, Pepsi, Sprite" },
                        new { Name = "Fresh Smoothie", Price = 9.50m, Description = "Seasonal fruit blend" },
                        new { Name = "Wine (Glass)", Price = 12.95m, Description = "House red or white" },
                        new { Name = "Beer (Domestic)", Price = 8.95m, Description = "Local brewery selection" },
                        new { Name = "Cocktail", Price = 14.95m, Description = "Premium mixed drinks" }
                    },
                    Amenity = new[]
                    {
                        new { Name = "Extra Towels", Price = 0m, Description = "Fresh bath and hand towels" },
                        new { Name = "Extra Blanket", Price = 0m, Description = "Soft cotton blanket" },
                        new { Name = "Extra Pillow", Price = 0m, Description = "Hypoallergenic pillow" },
                        new { Name = "Bathrobe", Price = 0m, Description = "Luxury terry cloth robe" },
                        new { Name = "Phone Charger", Price = 15.95m, Description = "Universal charging cables" },
                        new { Name = "Dental Kit", Price = 3.95m, Description = "Toothbrush and toothpaste" },
                        new { Name = "Shaving Kit", Price = 4.95m, Description = "Razor and shaving cream" },
                        new { Name = "Sewing Kit", Price = 2.95m, Description = "Basic repair supplies" }
                    },
                    Other = new[]
                    {
                        new { Name = "Laundry Service", Price = 25.00m, Description = "Per load, 24-hour service" },
                        new { Name = "Shoe Shine", Price = 8.00m, Description = "Professional shoe care" },
                        new { Name = "Newspaper Delivery", Price = 3.00m, Description = "Daily newspaper to room" },
                        new { Name = "Ice Delivery", Price = 0m, Description = "Bucket of ice" }
                    }
                };

                if (!string.IsNullOrEmpty(serviceType))
                {
                    if (!Enum.TryParse<RoomServiceType>(serviceType, out var serviceTypeEnum))
                    {
                        return Task.FromResult($"Invalid service type '{serviceType}'. Valid types are: Food, Beverage, Amenity, Other.");
                    }

                    var filteredItems = serviceTypeEnum switch
                    {
                        RoomServiceType.Food => menuItems.Food,
                        RoomServiceType.Beverage => menuItems.Beverage,
                        RoomServiceType.Amenity => menuItems.Amenity,
                        RoomServiceType.Other => menuItems.Other,
                        _ => null
                    };

                    return Task.FromResult(JsonSerializer.Serialize(new { ServiceType = serviceType, Items = filteredItems }, 
                        new JsonSerializerOptions { WriteIndented = true }));
                }

                return Task.FromResult(JsonSerializer.Serialize(menuItems, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                return Task.FromResult($"Error retrieving menu items: {ex.Message}");
            }
        }

        [KernelFunction]
        [Description("Get statistics about room service orders")]
        public async Task<string> GetRoomServiceStatsAsync(
            [Description("Number of days to look back for statistics (default: 30)")] int daysBack = 30)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-daysBack);
                
                var allOrders = await _context.RoomServices
                    .Where(rs => rs.RequestDate >= startDate)
                    .ToListAsync();

                if (!allOrders.Any())
                {
                    return JsonSerializer.Serialize(new { message = $"No room service orders found in the last {daysBack} days." });
                }

                var totalOrders = allOrders.Count;
                var totalRevenue = allOrders.Sum(rs => rs.Price);
                var averageOrderValue = allOrders.Average(rs => rs.Price);

                var ordersByStatus = allOrders
                    .GroupBy(rs => rs.Status)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

                var ordersByServiceType = allOrders
                    .GroupBy(rs => rs.ServiceType)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

                var popularItems = allOrders
                    .GroupBy(rs => rs.ItemName)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count());

                var deliveredOrders = allOrders
                    .Where(rs => rs.Status == RoomServiceStatus.Delivered && rs.DeliveryDate.HasValue)
                    .ToList();

                double avgDeliveryTimeMinutes = 0;
                if (deliveredOrders.Any())
                {
                    avgDeliveryTimeMinutes = deliveredOrders
                        .Average(rs => (rs.DeliveryDate!.Value - rs.RequestDate).TotalMinutes);
                }

                var result = new
                {
                    PeriodDays = daysBack,
                    StartDate = startDate,
                    EndDate = DateTime.UtcNow,
                    Statistics = new
                    {
                        TotalOrders = totalOrders,
                        TotalRevenue = totalRevenue,
                        AverageOrderValue = averageOrderValue,
                        OrdersByStatus = ordersByStatus,
                        OrdersByServiceType = ordersByServiceType,
                        PopularItems = popularItems
                    },
                    AverageDeliveryTimeMinutes = avgDeliveryTimeMinutes
                };

                return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = $"Error retrieving room service statistics: {ex.Message}" });
            }
        }

        public async Task<string> ProcessRoomServiceRequestAsync(string userMessage, string? conversationHistory = null)
        {
            try
            {
                // For now, let's handle simple cases directly
                var lowerMessage = userMessage.ToLower();
                
                if (lowerMessage.Contains("menu") || lowerMessage.Contains("what can i order"))
                {
                    return await GetMenuItemsAsync();
                }
                
                if (lowerMessage.Contains("my orders") || lowerMessage.Contains("order status"))
                {
                    return await SearchRoomServiceOrdersAsync();
                }
                
                if (lowerMessage.Contains("stats") || lowerMessage.Contains("statistics"))
                {
                    return await GetRoomServiceStatsAsync();
                }
                
                // Default response for other queries
                return JsonSerializer.Serialize(new 
                { 
                    message = "I can help you with room service. You can ask me to:",
                    options = new[]
                    {
                        "Show me the menu",
                        "Check my order status", 
                        "Create a new order",
                        "Get room service statistics"
                    }
                }, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = $"Error processing room service request: {ex.Message}" });
            }
        }
    }
}
