using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<RegPay>> GetTrips(int Id);
    Task<bool> ifExist(int id);
    
    Task<int> AddClient(ClientPOST addClient);
    
    Task<bool> AddClientToTrip(int ClientId, int TripId);
}