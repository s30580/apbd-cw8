using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using NuGet.Repositories;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;
using Microsoft.Data.SqlClient;


public class ClientsService : IClientsService
{
    private readonly string _connectionString =
        "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=master; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

    public async Task<List<RegPay>> GetTrips(int Id)
    {
        Dictionary<int, RegPay> map = new Dictionary<int, RegPay>();
        //zapytanie zwraca nam dane wycieczki, nazwę kraju oraz czas rejestracji i zapłaty
        string command =
            "SELECT clt.IdClient,t.IdTrip, t.Name,t.Description,t.DateFrom,t.DateTo,t.MaxPeople,c.Name AS CountryName, clt.RegisteredAt,clt.PaymentDate FROM Trip t JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip JOIN Country c ON ct.IdCountry = c.IdCountry JOIN Client_Trip clt ON clt.IdTrip = t.IdTrip WHERE clt.IdClient = @IdClient;";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", Id);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int tripId = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                    if (!map.ContainsKey(tripId))
                    {
                        map[tripId] = new RegPay()
                        {
                            Trip = new TripDTO()
                            {
                                Id = tripId,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                                DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                                MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                                Countries = new List<CountriesDTO>()
                            },
                            RegisterDate = RegPay.ParseDate(reader.GetInt32(reader.GetOrdinal("RegisteredAt"))),
                            PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate"))
                                ? null
                                : RegPay.ParseDate(reader.GetInt32(reader.GetOrdinal("PaymentDate")))

                        };
                    }

                    string countryName = reader.GetString(reader.GetOrdinal("CountryName"));
                    if (!map[tripId].Trip.Countries.Any(c => c.Name == countryName))
                    {
                        map[tripId].Trip.Countries.Add(new CountriesDTO
                        {
                            Name = countryName
                        });
                    }
                }
            }
        }

        return map.Values.ToList();
    }

    public async Task<bool> ifExist(int id)
    {
        //zapytanie zwraca nam 1 gry istnieje klient o danym id lub nic w przypadku jego braku
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand("SELECT 1 FROM Client WHERE IdClient = @Id", conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
    }

    public async Task<int> AddClient(ClientPOST addClient)
    {
        using var conn = new SqlConnection(_connectionString);
        //zapytanie wstawia nowego klienta o podanych danych i zwraca nam ostanie pole identity czyli pole idclient
        string comm =
            "Insert into Client (FirstName,LastName,Email,Telephone,Pesel) VALUES (@FirstName,@LastName,@Email,@Telephone,@Pesel) Select SCOPE_IDENTITY();";
        using (SqlCommand cmd = new SqlCommand(comm, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", addClient.FirstName);
            cmd.Parameters.AddWithValue("@LastName", addClient.LastName);
            cmd.Parameters.AddWithValue("@Email", addClient.Email);
            cmd.Parameters.AddWithValue("@Telephone", addClient.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", addClient.Pesel);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

    }

    public async Task<bool> AddClientToTrip(int ClientId, int TripId)
    {
        using var conn = new SqlConnection(_connectionString);
        //wstawia nowy rekord klienta i wycieczki wraz z obecną datą rejestracji do bazy danych
        string comm =
            "Insert into Client_Trip (IdClient,IdTrip,RegisteredAt,PaymentDate) VALUES (@IdClient,@IdTrip,@RegisteredAt,@PaymentDate)";
        using (SqlCommand cmd = new SqlCommand(comm, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", ClientId);
            cmd.Parameters.AddWithValue("@IdTrip", TripId);
            cmd.Parameters.AddWithValue("@RegisteredAt", int.Parse(DateTime.Now.ToString("yyyyMMdd")));
            cmd.Parameters.AddWithValue("@PaymentDate", DBNull.Value);
            try
            {
                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }

    public async Task<bool> CheckClientFromTrip(int ClientId, int TripId)
    {
        using var conn = new SqlConnection(_connectionString);
        //sprawdza i zwraca czy jest rekord z klientem i wycieczka o danym id 
        string comm = "Select * from Client_Trip where IdClient = @IdClient and IdTrip = @IdTrip";
        using (SqlCommand cmd = new SqlCommand(comm, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", ClientId);
            cmd.Parameters.AddWithValue("@IdTrip", TripId);

            try
            {
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public async Task<bool> RemoveClientFromTrip(int ClientId, int TripId)
    {
        using var conn = new SqlConnection(_connectionString);
        //usuwa rekord gdzie jest klient i wycieczka o podanym id
        string comm = "Delete from Client_Trip where IdClient = @IdClient and IdTrip = @IdTrip";
        using (SqlCommand cmd = new SqlCommand(comm, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", ClientId);
            cmd.Parameters.AddWithValue("@IdTrip", TripId);

            try
            {
                await conn.OpenAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
    