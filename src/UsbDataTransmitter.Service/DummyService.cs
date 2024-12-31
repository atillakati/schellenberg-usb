using UsbDataTransmitter.Service.Controllers;
using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service
{
    public class DummyService : ISchellenbergService
    {
        private readonly ILogger<DummyService> _logger;        

        public DummyService(ILogger<DummyService> logger)
        {
            _logger = logger;
            
        }
        public string Info => "Running dummy Schellengberg service implementation";

        public string DeviceName => "Dummy Schellengberg Service";

        public event EventHandler<SchellenbergEventArgs> EventReceived;        
        public event EventHandler<SchellenbergEventArgs> PairingMessageReceived;

        public void Down()
        {
            _logger.LogInformation("DummyService - Dummy goes down.");
            //EventReceived?.Invoke(this, new SchellenbergEventArgs { CurrentEvent = Events.StopReceived });
        }

        public void InitStick()
        {
            _logger.LogInformation("DummyService - Initializing...");

            Task.Run(() =>
            {
                Thread.Sleep(500);
            }).WaitAsync(TimeSpan.FromMilliseconds(2000));

            EventReceived?.Invoke(this, new SchellenbergEventArgs { CurrentEvent = Events.Started });
        }

        public void Pair()
        {            
        }

        public void Up()
        {
            _logger.LogInformation("DummyService - Dummy goes up.");
            //EventReceived?.Invoke(this, new SchellenbergEventArgs { CurrentEvent = Events.StopReceived });

        }
    }
}
