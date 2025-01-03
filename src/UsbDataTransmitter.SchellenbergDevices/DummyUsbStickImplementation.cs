using Microsoft.Extensions.Logging;

namespace UsbDataTransmitter.SchellenbergDevices
{
    public class DummyUsbStickImplementation : IUsbStick
    {
        private readonly ILogger<DummyUsbStickImplementation> _logger;

        public string DeviceInfo => "Dummy Usb Stick Implementation";

        public event EventHandler<UsbDataReceivedEventArgs> DataReceived;

        public DummyUsbStickImplementation(ILogger<DummyUsbStickImplementation> logger) 
        {
            _logger = logger;
        }

        public void Dispose()
        {
            
        }

        public int Write(string data)
        {
            _logger.LogInformation($".... Write {data}");
            return data.Length;
        }
    }
}
