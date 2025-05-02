using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly ITripsService _tripsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
            _tripsService = new TripsService();
        }
        //zwraca wycieczki dla klienta o danym id
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            if (!await _clientsService.ifExist(id))
            {
                return NotFound("Client not found");
            }
            var trips = await _clientsService.GetTrips(id);
            if (trips.Count == 0)
            {
                return Ok("Client has no trips");
            }
            return Ok(trips);
        }

        //dodaje nam nowego klienta do bazy danych i zwraca jego id
        [HttpPost]
        public async Task<IActionResult> PostTrip([FromBody] ClientPOST client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (client == null)
            {
                return BadRequest("Client is null");
            }

            try
            {
                var id = await _clientsService.AddClient(client);
                return StatusCode(201, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //dodaje nam klienta do wycieczki 
        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> PutTrip(int id, int tripId)
        {
            if (!await _clientsService.ifExist(id))
            {
                return NotFound("Client not found");
            }
            if (!await _tripsService.ifExist(tripId))
            {
                return NotFound("Trip not found");
            }

            if (!await _tripsService.ifMaxPeople(tripId))
            {
                return BadRequest("Max people taken");
            }
            
            bool res = await _clientsService.AddClientToTrip(id, tripId);
            if (!res)
            {
                return StatusCode(500,"Internal Server Error"); 
            }
            return StatusCode(201,"Client added to trip");
        }
        //usuwa nam klienta z bazy danych
        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> DeleteTrip(int id, int tripId)
        {
            if (!await _clientsService.CheckClientFromTrip(id, tripId))
            {
                return NotFound("Client or Trip not found");
            }
            var res = await _clientsService.RemoveClientFromTrip(id, tripId);
            if (!res)
            {
                return StatusCode(500, "Client was not removed");
            }
            return Ok("Client removed from trip");
        }
    }
}