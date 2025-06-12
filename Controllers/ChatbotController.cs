using Microsoft.AspNetCore.Mvc;
using CommunityToolkit.Diagnostics;
using ContosoHotels.Agents;

namespace ContosoHotels.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
  private readonly GuestAgent _guestAgent;
  public ChatbotController(GuestAgent guestAgent)
  {
    Guard.IsNotNull(guestAgent);
    _guestAgent = guestAgent;
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
      var response = await GenerateResponseAsync(request.Message);
      
      return Ok(new { message = response });
    }
    catch (Exception ex)
    {
      // Log the error (add your logging here)
      return StatusCode(500, new { message = "An error occurred while processing your request." });
    }
  }

  private async Task<string> GenerateResponseAsync(string userMessage)
  {
    var response = await _guestAgent.GetHotelInfoAsync(userMessage);
    return response;
  }
}

public class ChatbotRequest
{
    public string Message { get; set; } = string.Empty;
}
