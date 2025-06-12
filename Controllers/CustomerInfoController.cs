using Microsoft.AspNetCore.Mvc;
using CommunityToolkit.Diagnostics;
using ContosoHotels.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoHotels.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerInfoController : ControllerBase
{
  private readonly ContosoHotelsContext _db;
  public CustomerInfoController(ContosoHotelsContext db)
  {
    Guard.IsNotNull(db);
    _db = db;
  }

  [HttpGet("customer/{email}")]
  public async Task<IActionResult> GetCustomerInfo(string email)
  {
    try
    {
      var query = _db.Customers
        .Where(c => c.Email == email);

      var customers = await query.ToListAsync();

      var customer = customers.FirstOrDefault();

      if (customer == null)
      {
        return NotFound(new { message = "Customer not found." });
      }

      return Ok(customer);
    }
    catch (Exception ex)
    {
      // Log the error (add your logging here)
      return StatusCode(500, new { message = "An error occurred while retrieving customer information." });
    }
  }
}
