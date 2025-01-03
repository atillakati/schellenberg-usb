using Stateless;
using Stateless.Graph;
using System.Text;
using UsbDataTransmitter.SchellenbergDevices;
using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service.Services
{
    public class SchellenbergService : ISchellenbergService
    {
        private readonly ILogger<SchellenbergService> _logger;
        private readonly IUsbStick _usbStick;
        private readonly IDevice _device;
        private bool _isPaired;
        private StateMachine<States, Events> _fsm;


        public SchellenbergService(ILogger<SchellenbergService> logger, IUsbStick usbStick)
        {
            _logger = logger;

            _usbStick = usbStick;
            _usbStick.DataReceived += UsbStick_DataReceived;

            _device = new Device("265508", 0xA1, "Schellenberg Rollodrive Premium");
            _device.AddProperty(new DeviceProperty(_logger, "up", 0x01));
            _device.AddProperty(new DeviceProperty(_logger, "down", 0x02));
            _device.AddProperty(new DeviceProperty(_logger, "stop", 0x00));
            _device.AddProperty(new DeviceProperty(_logger, "pair", 0x60));

            //State Machine 
            SetupStateMachine();
            _fsm?.Activate();

            _logger.LogInformation("FSM startet. Current State: " + _fsm?.State);
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

        public States CurrentFsmState => _fsm.State;
                
        public void FireEvent(Events eventToFire)
        {
            _logger.LogInformation($"FireEvent - Current State: {_fsm.State} Event: {eventToFire}");
            _fsm?.Fire(eventToFire);
        }

        private async Task InitStick()
        {
            _logger.LogInformation("Initializing RF stick...");

            _usbStick.Write("!G");
            Thread.Sleep(200);
            _usbStick.Write("!?");
            Thread.Sleep(200);
            _usbStick.Write("hello");
            Thread.Sleep(200);
            _usbStick.Write("!?");

            Thread.Sleep(500);

            _fsm.Fire(Events.Started);
        }

        private void SetupStateMachine()
        {
            _fsm = new StateMachine<States, Events>(States.Unknown);
            _fsm.OnUnhandledTrigger((s, e) => _logger.LogWarning($"Unhandled trigger - State: {s} Event: {e}"));
            _fsm.OnTransitionCompleted((trans) => _logger.LogInformation($"FSM transition completed: {trans.Source} -> {trans.Trigger} => {trans.Destination}"));

            _fsm.Configure(States.Unknown)
                .OnActivate(() => _fsm.FireAsync(Events.Init))
                .Permit(Events.Init, States.Starting);

            _fsm.Configure(States.Starting)
                .OnEntryAsync(InitStick)
                .Permit(Events.Started, States.Idle);

            _fsm.Configure(States.Idle)
                .OnEntryFrom(Events.StopPressed, () => ExecuteCommand("stop"))
                .OnEntryFrom(Events.StopReceived, () => ExecuteCommand("stop"))
                .OnEntryFrom(Events.Paired, () => ExecuteCommand("stop"))
                .Permit(Events.MoveUpReceived, States.Moving)
                .Permit(Events.MoveDownReceived, States.Moving)
                .Permit(Events.MoveUpPressed, States.Moving)
                .Permit(Events.MoveDownPressed, States.Moving);

            _fsm.Configure(States.Idle).Permit(Events.PairingStartedReceived, States.Pairing);

            _fsm.Configure(States.Moving)
                .OnEntryFrom(Events.MoveUpReceived, () => ExecuteCommand("up"))
                .OnEntryFrom(Events.MoveUpPressed, () => ExecuteCommand("up"))
                .OnEntryFrom(Events.MoveDownReceived, () => ExecuteCommand("down"))
                .OnEntryFrom(Events.MoveDownPressed, () => ExecuteCommand("down"))
                .Permit(Events.StopReceived, States.Idle)
                .PermitReentry(Events.MoveDownPressed)
                .PermitReentry(Events.MoveDownReceived)
                .PermitReentry(Events.MoveUpPressed)
                .PermitReentry(Events.MoveUpReceived);

            _fsm.Configure(States.Pairing)
                .OnEntry(() => ExecuteCommand("pair"))
                .Permit(Events.Paired, States.Idle);

            //var graph = MermaidGraph.Format(_fsm.GetInfo());            
        }

        private void ExecuteCommand(string commandName)
        {
            _logger.LogInformation($"ExecuteCommand '{commandName}'...");

            var cmdProp = _device.Properties.FirstOrDefault(p => p.Name == commandName);
            if (cmdProp == null)
            {
                return;
            }

            var commandString = _device.CreateCommandString(cmdProp);
            _usbStick.Write(commandString);
        }

        private void UsbStick_DataReceived(object? sender, UsbDataReceivedEventArgs e)
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
                //PairingMessageReceived?.Invoke(this, new SchellenbergEventArgs { RawMessage = receivedData, Paired = _isPaired});

                //var bytesWritten = _usbStick.Write("ssA19600000");
                //if (bytesWritten > 0)
                //{
                //    _isPaired = true;
                //}
            }
            else
            {
                if (_device != null)
                {
                    _device.UpdateProperty(receivedData);
                }
            }
        }
    }
}
