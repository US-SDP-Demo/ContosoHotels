using Microsoft.AspNetCore.Mvc;
using ContosoHotels.Agents;

namespace ContosoHotels.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
  private readonly GuestAgent _guestAgent;
  private readonly RoomServiceSkAgent _roomServiceAgent;
  private readonly HousekeepingSkAgent _housekeepingAgent;
  
  public ChatbotController(GuestAgent guestAgent, RoomServiceSkAgent roomServiceAgent, HousekeepingSkAgent housekeepingAgent)
  {
    _guestAgent = guestAgent ?? throw new ArgumentNullException(nameof(guestAgent));
    _roomServiceAgent = roomServiceAgent ?? throw new ArgumentNullException(nameof(roomServiceAgent));
    _housekeepingAgent = housekeepingAgent ?? throw new ArgumentNullException(nameof(housekeepingAgent));
  }

  [HttpPost("message")]
  public async Task<IActionResult> ProcessMessage([FromBody] ChatbotRequest request)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(request.Message))
      {
        return BadRequest(new { message = "Message cannot be empty." });
      }

      // Process the user's message and generate a response
      var response = await GenerateResponseAsync(request);
      
      return Ok(new { message = response });
    }
    catch (Exception ex)
    {
      // Log the error
      Console.WriteLine($"Error processing message: {ex.Message}");
      return StatusCode(500, new { message = "An error occurred while processing your request." });
    }
  }

  [HttpPost("roomservice")]
  public async Task<IActionResult> ProcessRoomServiceMessage([FromBody] ChatbotRequest request)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(request.Message))
      {
        return BadRequest(new { message = "Message cannot be empty." });
      }

      // Process the room service message using the specialized agent
      var response = await _roomServiceAgent.ProcessRoomServiceRequestAsync(request.Message);
      
      return Ok(new { message = response });
    }
    catch (Exception ex)
    {
      // Log the error
      Console.WriteLine($"Error processing room service message: {ex.Message}");
      return StatusCode(500, new { message = "An error occurred while processing your room service request." });
    }
  }

  [HttpPost("housekeeping")]
  public async Task<IActionResult> ProcessHousekeepingMessage([FromBody] ChatbotRequest request)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(request.Message))
      {
        return BadRequest(new { message = "Message cannot be empty." });
      }

      // Process the housekeeping message using the specialized agent
      var response = await _housekeepingAgent.ProcessHousekeepingRequestAsync(request.Message);
      
      return Ok(new { message = response });
    }
    catch (Exception ex)
    {
      // Log the error
      Console.WriteLine($"Error processing housekeeping message: {ex.Message}");
      return StatusCode(500, new { message = "An error occurred while processing your housekeeping request." });
    }
  }

  private async Task<string> GenerateResponseAsync(string userMessage)
  {
    var response = await _guestAgent.GetHotelInfoAsync(request.GuestId, request.GuestName, request.Message);
    return response;
  }
}

public class ChatbotRequest
{
  public string Message { get; set; } = string.Empty;
  public string GuestName { get; set; } = string.Empty;
  public int GuestId { get; set; }
}
