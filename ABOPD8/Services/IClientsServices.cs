using ABOPD8.DPOs;
using ABOPD8.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABOPD8.Services;




public interface IClientsServices
{
    Task<IEnumerable<ClientTripDTO>> GetClientsTripsAsync(int id, CancellationToken cancellationToken);
    Task<int>AddClientAsync([FromBody] ClientDto clientDto, CancellationToken cancellationToken);
    Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken);
    Task<bool> IsClientRegisteredForTripAsync(int id, int tripId, CancellationToken cancellationToken);

    Task RegisterClientToTripAsync(int id, int tripId, CancellationToken cancellationToken);
    Task DeleteClientAsync(int id, int tripId, CancellationToken cancellationToken);
    Task<bool> ClientHasTripsAsync(int clientId, CancellationToken cancellationToken);
}