using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        Dictionary<int, TripDTO> dic = new Dictionary<int, TripDTO>();
        string command = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName " +
                         "FROM Trip t " +
                         "JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip " +
                         "JOIN Country c ON ct.IdCountry = c.IdCountry;";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idTrip = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                    if (!dic.ContainsKey(idTrip))
                    {
                        dic[idTrip] = new TripDTO()
                        {
                            Id = idTrip,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = new List<CountriesDTO>()
                        };
                    }
                    string countryName = reader.GetString(reader.GetOrdinal("CountryName"));
                    if (!dic[idTrip].Countries.Any(c => c.Name == countryName))
                    {
                        dic[idTrip].Countries.Add(new CountriesDTO
                        {
                            Name = countryName
                        });
                    }
                }
            }
        }
        return dic.Values.ToList();
    }
}