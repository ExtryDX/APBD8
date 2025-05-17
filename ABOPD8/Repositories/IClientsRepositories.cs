using ABOPD8.DPOs;
using Microsoft.AspNetCore.Mvc;

namespace ABOPD8.Repositories;

public interface IClientsRepositories
{
    Task<IEnumerable<ClientTripDTO>> GetClientsTripsAsync(int id, CancellationToken cancellationToken);
    Task<int> AddClientAsync([FromBody] ClientDto clientDto, CancellationToken cancellationToken);
    Task<bool> ClientExists(int id, CancellationToken cancellationToken);
    Task<bool> IsClientRegisteredForTripAsync(int id, int tripId, CancellationToken cancellationToken);

    Task RegisterClientToTripAsync(int id, int tripId, CancellationToken cancellationToken);
    Task DeleteClientAsync(int id, int tripId, CancellationToken cancellationToken);
    
    Task<bool> ClinetHasTripsAsync(int id, CancellationToken cancellationToken);
}