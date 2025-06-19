using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ContosoHotels.Agents;

namespace ContosoHotels.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
  private readonly OrchestratorService _orchestratorService;
  
  public ChatbotController(OrchestratorService orchestratorService)
  {
    Guard.IsNotNull(orchestratorService);
    _orchestratorService = orchestratorService;
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
      var response = await _orchestratorService.HandleGuestQueryAsync(request.GuestId, request.GuestName, request.Message);
      
      return Ok(new { message = response });
    }
    catch (Exception ex)
    {
      // Log the error
      Console.WriteLine($"Error processing message: {ex.Message}");
      return StatusCode(500, new { message = "An error occurred while processing your request." });
    }
  }
}

public class ChatbotRequest
{
  public string Message { get; set; } = string.Empty;
  public string GuestName { get; set; } = string.Empty;
  public int GuestId { get; set; }
}
