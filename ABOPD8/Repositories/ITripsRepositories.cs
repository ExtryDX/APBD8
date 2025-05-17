using ABOPD8.DPOs;

namespace ABOPD8.Repositories;

public interface ITripsRepositories
{
    Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken);
    Task<bool> TripExistsAsync(int tripId, CancellationToken cancellationToken);
    
    Task<int> SeatsTakenAsync(int tripId, CancellationToken cancellationToken);
    Task<int> GetMaxSeatsAsync(int tripId, CancellationToken cancellationToken);
}