using ABOPD8.DPOs;
using ABOPD8.Models;
using Microsoft.Data.SqlClient;

namespace ABOPD8.Repositories;

public class TripsRepositories : ITripsRepositories
{
    private readonly string _connectionString;

    public TripsRepositories()
    {
        _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
    }
    
    public async Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken)
    {
        var tripsDict = new Dictionary<int, TripDTO>();
        await using (var connect = new SqlConnection(_connectionString))
        {
            await using var sqlCommand = new SqlCommand();
            sqlCommand.Connection = connect;
            sqlCommand.CommandText = @"SELECT 
                T.IdTrip,
                T.Name AS TripName,
                T.Description,
                T.DateFrom,
                T.DateTo,
                T.MaxPeople,
                C.IdCountry,
                C.Name AS CountryName
            FROM Trip T
            JOIN Country_Trip CT ON T.IdTrip = CT.IdTrip
            JOIN Country C ON CT.IdCountry = C.IdCountry";

            await connect.OpenAsync(cancellationToken);

            var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);
            var trips = new List<TripDTO>();


            while (await reader.ReadAsync(cancellationToken))
            {

                int idTrip = (int)reader["IdTrip"];

                if (!tripsDict.ContainsKey(idTrip))
                {
                    var tripDPO = new TripDTO
                    {
                        IdTrip = idTrip,
                        Name = (string)reader["TripName"],
                        Description = (string)reader["Description"],
                        DateFrom = (DateTime)reader["DateFrom"],
                        DateTo = (DateTime)reader["DateTo"],
                        MaxPeople = (int)reader["MaxPeople"],
                        Countries = new List<Country>()
                    };

                    tripsDict.Add(idTrip, tripDPO);
                    var country = new Country
                    {
                        IdCountry = (int)reader["IdCountry"],
                        Name = (string)reader["CountryName"],
                    };

                    tripsDict[idTrip].Countries.Add(country);

                }

            }

        }

        return tripsDict.Values.ToList();
    }



    public async Task<bool> TripExistsAsync(int tripId, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            await using var existCom = new SqlCommand("SELECT 1 FROM trip WHERE idTrip = @tripId", connect);
            existCom.Parameters.AddWithValue("@tripId", tripId);
            await connect.OpenAsync(cancellationToken);
            var exists = await existCom.ExecuteScalarAsync(cancellationToken);
            return exists != null;
        }
    }

    public async Task<int> SeatsTakenAsync(int tripId, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            await using var command = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @tripId;", connect);
            command.Parameters.AddWithValue("@tripId", tripId);
            await connect.OpenAsync(cancellationToken);
            var seatsTaken = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
            return seatsTaken;
        }
    }
    
    public async Task<int> GetMaxSeatsAsync(int tripId, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            await using var command = new SqlCommand("SELECT maxPeople FROM Trip WHERE IdTrip = @tripId;", connect);
            command.Parameters.AddWithValue("@tripId", tripId);
            await connect.OpenAsync(cancellationToken);
            var maxSeats = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
            return maxSeats;
        }
    }
}