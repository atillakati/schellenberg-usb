using Stateless;
using Stateless.Graph;
ususing System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;
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
                private double _fullTravelTime = 16.0;
        private int _currentPosition = 0;
        private DateTime? _movementStart = null;
        private int _targetPosition = 0;
        private string _currentDirection = null;
        private double _fullTravelTimeUp = 16.0;
        private double _fullTravelTimeDown = 16.0;
        private TaskCompletionSource<bool> _calibrationStopTcs;




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
                    info = "Device not ready.";
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
       {
    _logger.LogInformation($"FireEvent - Current State: {_fsm.State} Event: {eventToFire}");

    // Aktuelle Bewegung setzen oder Position nach Stop berechnen
    switch (eventToFire)
    {
        case Events.MoveUpReceived:
            _movementStart = DateTime.Now;
            _currentDirection = "up";
            break;
        case Events.MoveDownReceived:
            _movementStart = DateTime.Now;
            _currentDirection = "down";
            break;
        case Events.StopReceived:
        case Events.StopPressed:
            if (_movementStart.HasValue)
            {
                var elapsed = DateTime.Now - _movementStart.Value;
                double denominator;
                if (_currentDirection == "down")
                {
                    denominator = _fullTravelTimeDown > 0 ? _fullTravelTimeDown : _fullTravelTime;
                }
                else if (_currentDirection == "up")
                {
                    denominator = _fullTravelTimeUp > 0 ? _fullTravelTimeUp : _fullTravelTime;
                }
                else
                {
                    denominator = _fullTravelTime;
                }
                var deltaPercent = elapsed.TotalSeconds / denominator * 100.0;
                if (_targetPosition.HasValue)
                {
                    _currentPosition = _targetPosition.Value;
                    _targetPosition = null;
                }
                else if (_currentDirection == "down")
                {
                    _currentPosition = Math.Min(100, _currentPosition + (int)Math.Round(deltaPercent));
                }
                else if (_currentDirection == "up")
                {
                    _currentPosition = Math.Max(0, _currentPosition - (int)Math.Round(deltaPercent));
                }
                _movementStart = null;
                _currentDirection = null;
            }
            _calibrationStopTcs?.TrySetResult(true);
            break;
    }
    _fsm?.Fire(eventToFire);
}    _logger.LogInformation($"FireEvent - Current State: {_fsm.State} Event: {eventToFire}");
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
                .Permit(Events.MoveUpReceived, States.Opening)
                .Permit(Events.MoveDownReceived, States.Closing)
                .Permit(Events.MoveUpPressed, States.Opening)
                .Permit(Events.MoveDownPressed, States.Closing);

            _fsm.Configure(States.Idle).Permit(Events.PairingStartedReceived, States.Pairing);

            _fsm.Configure(States.Opening)
                .OnEntryFrom(Events.MoveUpReceived, () => ExecuteCommand("up"))
                .OnEntryFrom(Events.MoveUpPressed, () => ExecuteCommand("up"))                
                .Permit(Events.StopReceived, States.Open)
                .Permit(Events.StopPressed, States.Open)
                .PermitReentry(Events.MoveUpPressed)
                .PermitReentry(Events.MoveUpReceived);

            _fsm.Configure(States.Open)
                .OnEntryFrom(Events.StopReceived, () => ExecuteCommand("stop"))
                .OnEntryFrom(Events.StopPressed, () => ExecuteCommand("stop"))
                .Permit(Events.MoveDownPressed, States.Closing)
                .Permit(Events.MoveDownReceived, States.Closing)
                .Permit(Events.MoveUpPressed, States.Opening)
                .Permit(Events.MoveUpReceived, States.Opening)
                .Permit(Events.PairingStartedReceived, States.Pairing)
                .PermitReentry(Events.StopPressed)
                .PermitReentry(Events.StopReceived);

            _fsm.Configure(States.Closed)
                .OnEntryFrom(Events.StopReceived, () => ExecuteCommand("stop"))
                .OnEntryFrom(Events.StopPressed, () => ExecuteCommand("stop"))
                .Permit(Events.MoveDownPressed, States.Closing)
                .Permit(Events.MoveDownReceived, States.Closing)
                .Permit(Events.MoveUpPressed, States.Opening)
                .Permit(Events.MoveUpReceived, States.Opening)
                .Permit(Events.PairingStartedReceived, States.Pairing)
                .PermitReentry(Events.StopPressed)
                .PermitReentry(Events.StopReceived);

            _fsm.Configure(States.Closing)                
                .OnEntryFrom(Events.MoveDownReceived, () => ExecuteCommand("down"))
                .OnEntryFrom(Events.MoveDownPressed, () => ExecuteCommand("down"))
                .Permit(Events.StopReceived, States.Closed)
                .Permit(Events.StopPressed, States.Closed)
                .PermitReentry(Events.MoveDownPressed)
                .PermitReentry(Events.MoveDownReceived);

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

