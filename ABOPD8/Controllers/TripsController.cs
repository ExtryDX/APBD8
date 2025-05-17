using ABOPD8.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABOPD8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly  ITripsServices _tripsServices;

    public TripsController(ITripsServices tripsServices)
    {
        _tripsServices = tripsServices;
    }
    
    //Endpoint GET /api/trips zwracający informacje o wycieczkach
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        var data = await _tripsServices.GetTripsAsync(cancellationToken);
        return Ok(data);
    }
}