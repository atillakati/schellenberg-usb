using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //public void Init()
        //{
        //    _logger.LogInformation("Starting with dummy init sequence..");
        //}
    }
}
