using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsbDataTransmitter.Service.Entities;
using UsbDataTransmitter.Service.Services;
using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchellenbergController : ControllerBase
    {
        private const string VERSION = "0.2.0";
        private readonly ILogger<SchellenbergController> _logger;
        private readonly ISchellenbergService _schellenbergService;

        public SchellenbergController(ILogger<SchellenbergController> logger,
                                      ISchellenbergService schellenbergService)
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
                version = VERSION,
                fsm_state = _schellenbergService.CurrentFsmState.ToString(),
                position = _schellenbergService.CurrentPosition
            };
        }

        [HttpGet("{direction}")]
        public DeviceInfo Move(string direction)
        {
            _logger.LogInformation("Move() called.");

            switch (direction)
            {
                case "up":
                    _schellenbergService.FireEvent(Events.MoveUpReceived);
                    break;
                case "down":
                    _schellenbergService.FireEvent(Events.MoveDownReceived);
                    break;
                case "stop":
                    _schellenbergService.FireEvent(Events.StopReceived);
                    break;
                case "pair":
                    _schellenbergService.FireEvent(Events.PairingStartedReceived);
                    break;
            }
            return new DeviceInfo
            {
                lastUpdate = DateTime.Now,
                message = $"Move {direction}",
                name = _schellenbergService.DeviceName,
                version = VERSION,
                fsm_state = _schellenbergService.CurrentFsmState.ToString(),
                position = _schellenbergService.CurrentPosition
            };
        }

        /// <summary>
        /// Fährt den Gurtwickler auf die angegebene prozentuale Position.
        /// Aufruf: GET /schellenberg/moveTo/{percent}
        /// </summary>
        [HttpGet("moveTo/{percent}")]
        public async Task<DeviceInfo> MoveTo(int percent)
        {
            _logger.LogInformation("MoveTo() called.");
            await _schellenbergService.MoveToAsync(percent);
            return new DeviceInfo
            {
                lastUpdate = DateTime.Now,
                message = $"MoveTo {percent}",
                name = _schellenbergService.DeviceName,
                version = VERSION,
                fsm_state = _schellenbergService.CurrentFsmState.ToString(),
                position = _schellenbergService.CurrentPosition
            };
        }
    }
}
