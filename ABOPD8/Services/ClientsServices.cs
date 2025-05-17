using ABOPD8.DPOs;
using ABOPD8.Models;
using ABOPD8.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ABOPD8.Services;

public class ClientsServices : IClientsServices
{
    private readonly IClientsRepositories _iClientsRepositories;
    private readonly string _connectionString;

    public ClientsServices(IClientsRepositories iClientsRepositories)
    {
        _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        _iClientsRepositories = iClientsRepositories;
    }

    public async Task<bool> ClientHasTripsAsync(int clientId, CancellationToken cancellationToken)
    {
        var clientTripExist = await _iClientsRepositories.ClinetHasTripsAsync(clientId, cancellationToken);
        return clientTripExist;
    }
    
    
    async public Task<IEnumerable<ClientTripDTO>> GetClientsTripsAsync(int id, CancellationToken cancellationToken)
    {
        
        return await _iClientsRepositories.GetClientsTripsAsync(id, cancellationToken);
    }
    
    public async Task<int> AddClientAsync([FromBody] ClientDto clientDto, CancellationToken cancellationToken)
    {
        if (clientDto == null) throw new ArgumentNullException(nameof(clientDto));
        return await _iClientsRepositories.AddClientAsync(clientDto, cancellationToken);
    }

    public async Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _iClientsRepositories.ClientExists(id, cancellationToken);
    }

    public async Task<bool> IsClientRegisteredForTripAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        return await _iClientsRepositories.IsClientRegisteredForTripAsync(id, tripId, cancellationToken);
    }

    public async Task RegisterClientToTripAsync(int id, int tripId, CancellationToken cancellationToken)
    {
         await _iClientsRepositories.RegisterClientToTripAsync(id, tripId, cancellationToken);
    }

    public async Task DeleteClientAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        await _iClientsRepositories.DeleteClientAsync(id, tripId, cancellationToken);
    }
}