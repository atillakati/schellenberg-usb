using Microsoft.AspNetCore.Mvc;
using UsbDataTransmitter.Service.Entities;
using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchellenbergController : ControllerBase
    {
        private readonly ILogger<SchellenbergController> _logger;
        private readonly ISchellenbergService _schellenbergService;
        private readonly IStateMachine _stateMachine;

        public SchellenbergController(ILogger<SchellenbergController> logger, ISchellenbergService schellenbergService, IStateMachine stateMachine)
        {
            _logger = logger;
            _schellenbergService = schellenbergService;
            _stateMachine = stateMachine;
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

            switch(direction)
            {
                case "up":
                    _stateMachine.FireEvent(Events.MoveUpReceived);
                    break;

                case "down":
                    _stateMachine.FireEvent(Events.MoveDownReceived);
                    break;

                case "stop":
                    _stateMachine.FireEvent(Events.StopReceived);
                    break;

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
