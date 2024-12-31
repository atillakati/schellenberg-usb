using UsbDataTransmitter.Service.Controllers;
using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service
{
    public class DummyService : ISchellenbergService
    {
        private readonly ILogger<SchellenbergController> _logger;        

        public DummyService(ILogger<SchellenbergController> logger)
        {
            _logger = logger;
            
        }
        public string Info => "Running dummy Schellengberg service implementation";

        public string DeviceName => "Dummy Schellengberg Service";

        public event EventHandler<SchellenbergEventArgs> UpMessageReceived;
        public event EventHandler<SchellenbergEventArgs> DownMessageReceived;
        public event EventHandler<SchellenbergEventArgs> StopMessageReceived;
        public event EventHandler<SchellenbergEventArgs> PairingMessageReceived;

        public void Down()
        {
            _logger.LogInformation("DummyService - Dummy goes down.");           
        }

        public async Task InitStick()
        {
            _logger.LogInformation("DummyService - Initializing...");            
        }

        public void Pair()
        {
            throw new NotImplementedException();
        }

        public void Up()
        {
            _logger.LogInformation("DummyService - Dummy goes up.");            
        }
    }
}
