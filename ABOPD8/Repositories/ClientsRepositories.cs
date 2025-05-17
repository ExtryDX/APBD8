using ABOPD8.DPOs;
using ABOPD8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ABOPD8.Repositories;

public class ClientsRepositories : IClientsRepositories
{
    private readonly string _connectionString;

    public ClientsRepositories()
    {
        
        _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
    }
    
    async public Task<IEnumerable<ClientTripDTO>> GetClientsTripsAsync(int id, CancellationToken cancellationToken)
    {
        
        var clientTripList = new List<ClientTripDTO>();
        await using (var connect = new SqlConnection(_connectionString))
        {
            await using var sqlCommand = new SqlCommand();
            sqlCommand.Connection = connect;
            connect.OpenAsync(cancellationToken);
            
            //Poberz dane dotoyczace wycieczki
            sqlCommand.CommandText = @"SELECT T.IdTrip, T.Name, T.Description, T.DateFrom, T.DateTo, T.MaxPeople,
                CT.RegisteredAt, CT.PaymentDate
                FROM Trip T
                JOIN Client_Trip CT ON T.IdTrip = CT.IdTrip
                WHERE CT.IdClient = @id";

            sqlCommand.Parameters.AddWithValue("@id", id);

            var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);
            var clientsTrips = new List<TripDTO>();

            while (await reader.ReadAsync(cancellationToken))
            {

                var clientTripDTO = new ClientTripDTO
                {
                    IdTrip = (int)reader["IdTrip"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"],
                    DateFrom = (DateTime)reader["DateFrom"],
                    DateTo = (DateTime)reader["DateTo"],
                    MaxPeople = (int)reader["MaxPeople"],
                    RegisteredAt = (int)reader["RegisteredAt"],
                    PaymentDate = (int)reader["PaymentDate"]
                };

                clientTripList.Add(clientTripDTO);
            }
        }
        return clientTripList;
    }
    
    public async Task<int> AddClientAsync([FromBody] ClientDto clientDto, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            await using var sqlCommand = new SqlCommand();
            sqlCommand.Connection = connect;
            
            sqlCommand.CommandText =
                @"INSERT INTO Client(FirstName, LastName, Email, Telephone, Pesel) OUTPUT INSERTED.IdClient VALUES ( @FirstName, @LastName, @Email, @Telephone, @Pesel)";
            
            sqlCommand.Parameters.AddWithValue("@FirstName", clientDto.FirstName);
            sqlCommand.Parameters.AddWithValue("@LastName", clientDto.LastName);
            sqlCommand.Parameters.AddWithValue("@Email", clientDto.Email);
            sqlCommand.Parameters.AddWithValue("@Telephone", clientDto.Telephone);
            sqlCommand.Parameters.AddWithValue("@Pesel", clientDto.Pesel);
            
            await connect.OpenAsync(cancellationToken);

            var result = await sqlCommand.ExecuteScalarAsync(cancellationToken);

            return (int)result;
        }
    }

    public async Task<bool> ClientExists(int id, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            //czy klient o podanym id ma zarejestrowaną wycieczkę
            await using var existCom = new SqlCommand("SELECT 1 FROM Client_Trip WHERE idClient = @id", connect);
            existCom.Parameters.AddWithValue("@id", id);
            await connect.OpenAsync(cancellationToken);
            var exists = await existCom.ExecuteScalarAsync(cancellationToken);
            return exists != null;
        }
    }

    public async Task<bool> IsClientRegisteredForTripAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            //Czy czy klient o podanym id ma przypisaną wcieczkę od podanym id wycieczki
            await using var command = new SqlCommand("SELECT 1 FROM Client_Trip WHERE idClient = @id AND idTrip = @tripId", connect);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@tripId", tripId);
            await connect.OpenAsync(cancellationToken);
            var registered = (int)(await command.ExecuteScalarAsync(cancellationToken)??0);
            return registered != 0;
        }
    }


    public async Task RegisterClientToTripAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            //Dodaj dane dotyczące rejestracji klienta na wycieczkę
            await using var command = new SqlCommand(@"
                                                    INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
                                                    VALUES (@clientId, @tripId, @registeredAt)", connect);
            command.Parameters.AddWithValue("@clientId", id);
            command.Parameters.AddWithValue("@tripId", tripId);
            command.Parameters.AddWithValue("@registeredAt", int.Parse(DateTime.Now.ToString("yyyyMMdd")));
            await connect.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task DeleteClientAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            //usuń rejestracje na wycieczkę dla podnacyh id klienta i wycieczki
            await using var command = new SqlCommand(@"
                                                    DELETE FROM Client_Trip
                                                    WHERE IdCLient = @clientId AND IdTrip = @tripId;", connect);
            command.Parameters.AddWithValue("@clientId", id);
            command.Parameters.AddWithValue("@tripId", tripId);
            await connect.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<bool> ClinetHasTripsAsync(int id, CancellationToken cancellationToken)
    {
        await using (var connect = new SqlConnection(_connectionString))
        {
            //Czy klient jest zarejestroway na jakąś wycieczkę
            await using var command= new SqlCommand("SELECT 1 FROM Client_Trip WHERE idClient = @id", connect);
            command.Parameters.AddWithValue("@id", id);
            await connect.OpenAsync(cancellationToken);
            var exist = (int)(await command.ExecuteScalarAsync(cancellationToken)??0);
            return exist != 0;
        }
    }
}