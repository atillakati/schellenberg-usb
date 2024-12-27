using Microsoft.AspNetCore.Mvc;
using UsbDataTransmitter.Service.Entities;

namespace UsbDataTransmitter.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchellenbergController : ControllerBase
    {
        private readonly ILogger<SchellenbergController> _logger;
        private readonly ISchellenbergService _schellenbergService;        

        public SchellenbergController(ILogger<SchellenbergController> logger, ISchellenbergService schellenbergService)
        {
            _logger = logger;
            _schellenbergService = schellenbergService;
        }
        

        [HttpGet]
        public DeviceInfo Get()
        {
            _logger.LogInformation("Get() called.");

            return new DeviceInfo
            {
                lastUpdate = DateTime.Now,
                message = _schellenbergService.Info,
                name = _schellenbergService.DeviceName,
                version = "0.1"
            };
        }

        [HttpGet("{direction}")]        
        public DeviceInfo Move(string direction)
        {
            _logger.LogInformation("Move() called.");

            if (direction == "up")
            {
                _schellenbergService.Up();
            }
            else if(direction == "down")
            {
                _schellenbergService.Down();
            }

            return new DeviceInfo
            {
                lastUpdate = DateTime.Now,
                message = $"Move {direction}",
                name = _schellenbergService.DeviceName,
                version = "0.1"
            };
        }        
    }
}
