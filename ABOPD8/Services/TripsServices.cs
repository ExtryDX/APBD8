using ABOPD8.DPOs;
using ABOPD8.Models;
using ABOPD8.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace ABOPD8.Services;

public class TripsServices : ITripsServices
{
    private readonly string _connectionString;
    private readonly ITripsRepositories _tripsRepositories;

    public TripsServices(ITripsRepositories tripsRepositories)
    {
        _connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        _tripsRepositories = tripsRepositories;
    }

    public async Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken)
    {
        return await _tripsRepositories.GetTripsAsync(cancellationToken);
    }

    public async Task<bool> TripExistsAsync(int tripId, CancellationToken cancellationToken)
    {
        return await _tripsRepositories.TripExistsAsync(tripId, cancellationToken);
    }
    public async Task<bool> AreSeatsAvaiableAsync(int tripId, CancellationToken cancellationToken)
    {
        var maxSeats = await _tripsRepositories.GetMaxSeatsAsync(tripId, cancellationToken);
        var seatsTaken = await _tripsRepositories.SeatsTakenAsync(tripId, cancellationToken);
        return (maxSeats - seatsTaken) > 0;
    }
}