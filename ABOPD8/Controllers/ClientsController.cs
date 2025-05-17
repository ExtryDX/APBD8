using ABOPD8.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ABOPD8.DPOs;

namespace ABOPD8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientsServices _clientsServices;
    private readonly ITripsServices _tripsServices;
    
    public ClientsController(IClientsServices clientsServices, ITripsServices tripsServices)
    {
        _clientsServices = clientsServices;
        _tripsServices = tripsServices;
    }
    
    //Endpoint GET /api/trips zwracający wycieczki wraz z ich podstawowymi informacjam
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientsAsync(int id, CancellationToken cancellationToken)
    {
        
        var clientExist = await _clientsServices.ClientExistsAsync(id, cancellationToken);
        if (!clientExist)
        {
            return NotFound("Client not found");
        }
        
        var clientHasTrips = await _clientsServices.ClientHasTripsAsync(id, cancellationToken);
        if (!clientHasTrips)
        {
            return NotFound("Nie znaleziono wycieczek dla klienta o podanym id");
        }
        
        var data = await _clientsServices.GetClientsTripsAsync(id, cancellationToken);
        return Ok(data);
    }

    
    //Endpoint POST /api/clients dodający do bazy nowych klientów
    [HttpPost]
    public async Task<IActionResult> AddClientAsync([FromBody] ClientDto clientDto, CancellationToken cancellationToken)
    {
        var dane = await _clientsServices.AddClientAsync(clientDto, cancellationToken);
        return Ok(dane);
    }

    
    //Endpoint PUT /api/clients/{id}/trips/{tripId} rejestrujący wyczieczkę dla użytkownika
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> UpdateClientAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        var clientExists = await _clientsServices.ClientExistsAsync(id, cancellationToken);
        if (!clientExists)
            return NotFound($"Client with ID {id} not found.");
        
        var tripExists = await _tripsServices.TripExistsAsync(tripId, cancellationToken);
        if (!tripExists)
            return NotFound($"Trip with ID {tripId} not found.");
        
        var areSeatsAvailable = await _tripsServices.AreSeatsAvaiableAsync(tripId, cancellationToken);
        if (!areSeatsAvailable)
            return BadRequest("Maximum number of participants for this trip has been reached.");
        
        var alreadyRegistered = await _clientsServices.IsClientRegisteredForTripAsync(id, tripId, cancellationToken);
        if (alreadyRegistered)
            return Conflict("Client is already registered for this trip.");
        
        await _clientsServices.RegisterClientToTripAsync(id, tripId, cancellationToken);
        
        return Ok("Client successfully registered for the trip.");
    }

    
    //Endpoint DELETE /api/clients/{id}/trips/{tripId} usuwający wycieczkę dla podanego id klienta
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> DeleteClientAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        var clientExists = await _clientsServices.ClientExistsAsync(id, cancellationToken);
        if (!clientExists)
        {
            return NotFound($"Client with ID {id} not found.");
        }
        
        var tripExists = await _tripsServices.TripExistsAsync(tripId, cancellationToken);
        if (!tripExists)
        {
            return NotFound($"Trip with ID {tripId} not found.");
        }
        
        await _clientsServices.DeleteClientAsync(id, tripId, cancellationToken);
        
        return Ok("Client successfully deleted.");
    }
    
}