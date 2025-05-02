using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTrips();
    Task<bool> ifExist(int id);
    Task<bool> ifMaxPeople(int id);
}