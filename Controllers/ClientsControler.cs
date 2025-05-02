using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            var trips = await _clientsService.GetTrips(id);
            return Ok(trips);
        }
    }
}