using ABOPD8.DPOs;
using ABOPD8.Models;

namespace ABOPD8.Services;

public interface ITripsServices
{
    Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken);
    
    Task<bool> TripExistsAsync(int tripId, CancellationToken cancellationToken);
    Task<bool> AreSeatsAvaiableAsync(int tripId, CancellationToken cancellationToken);

}