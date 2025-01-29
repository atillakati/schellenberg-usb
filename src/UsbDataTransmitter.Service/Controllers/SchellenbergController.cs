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
        private const string VERSION = "0.1.0";
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
                fsm_state = _schellenbergService.CurrentFsmState.ToString()
            };
        }

        [HttpGet("{direction}")]        
        public DeviceInfo Move(string direction)
        {
            _logger.LogInformation("Move() called.");

            switch(direction)
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

                //case "init":
                //    _schellenbergService.Init();
                //    break;

                //case "end":
                //    _schellenbergService.Close();
                //    break;

            }
            
            return new DeviceInfo
            {
                lastUpdate = DateTime.Now,
                message = $"Move {direction}",
                name = _schellenbergService.DeviceName,
                version = VERSION,
                fsm_state = _schellenbergService.CurrentFsmState.ToString() 
            };
        }        
    }
}
