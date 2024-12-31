using System.Text;
using UsbDataTransmitter.SchellenbergDevices;

namespace UsbDataTransmitter.Service.Controllers
{
    public class SchellenbergService : ISchellenbergService
    {
        private readonly ILogger<SchellenbergController> _logger;
        private readonly IUsbStick _usbStick;
        private readonly IDevice _device;
        private bool _isPaired;

        public event EventHandler<SchellenbergEventArgs> EventReceived;        
        public event EventHandler<SchellenbergEventArgs> PairingMessageReceived;

        public SchellenbergService(ILogger<SchellenbergController> logger)
        {
            _logger = logger;

            _usbStick = new UsbStick(logger);
            _usbStick.DataReceived += _usbStick_DataReceived;
            
            _device = new Device("265508", 0xA1, "Schellenberg Rollodrive Premium");
            _device.AddProperty(new DeviceProperty(_logger, "up", 0x01));
            _device.AddProperty(new DeviceProperty(_logger, "down", 0x02));
            _device.AddProperty(new DeviceProperty(_logger, "pair", 0x60));
        }

        public string Info
        {
            get
            {
                var info = _usbStick.DeviceInfo;
                if (string.IsNullOrEmpty(info))
                {
                    return "Device not ready.";
                }

                return info;
            }
        } 

        public string DeviceName
        {
            get
            {
                return _device.Name;
            }
        }

        public void Up()
        {
            ExecuteCommand("up");
        }        

        public void Down()
        {
            ExecuteCommand("down");
        }

        public void Pair()
        {
            ExecuteCommand("pair");
            _isPaired = true;

            PairingMessageReceived?.Invoke(this, new SchellenbergEventArgs { RawMessage = string.Empty, Paired = _isPaired });
        }

        public void InitStick()
        {
            _logger.LogInformation("Initializing RF stick...", MessageType.General);

            Task.Run(() =>
            {

                _usbStick.Write("!G");
                Thread.Sleep(200);
                _usbStick.Write("!?");
                Thread.Sleep(200);
                _usbStick.Write("hello");
                Thread.Sleep(200);
                _usbStick.Write("!?");
            });

            EventReceived?.Invoke(this, new SchellenbergEventArgs { CurrentEvent = StateMachineTypes.Events.Started });
        }


        private void ExecuteCommand(string commandName)
        {
            var cmdProp = _device.Properties.FirstOrDefault(p => p.Name == commandName);
            if (cmdProp == null)
            {
                return;
            }

            var commandString = _device.CreateCommandString(cmdProp);
            _usbStick.Write(commandString);

            _logger.LogInformation($"Command {commandName} => '{commandString}' send to device.");
        }

        private void _usbStick_DataReceived(object? sender, UsbDataReceivedEventArgs e)
        {
            if (!e.Count.HasValue || e.Buffer == null)
            {
                return;
            }

            var receivedData = Encoding.ASCII.GetString(e.Buffer, 0, e.Count.Value);
            _logger.LogInformation(receivedData, MessageType.Receive);

            //pairing
            if (receivedData.StartsWith("sl") && !_isPaired)
            {
                PairingMessageReceived?.Invoke(this, new SchellenbergEventArgs { RawMessage = receivedData, Paired = _isPaired});

                //var bytesWritten = _usbStick.Write("ssA19600000");
                //if (bytesWritten > 0)
                //{
                //    _isPaired = true;
                //}
            }
            else
            {
                if(_device != null)
                {
                    _device.UpdateProperty(receivedData);
                }                
            }
        }        
    }
}
