using Microsoft.Extensions.Logging;
using Stateless;
using Stateless.Graph;
using UsbDataTransmitter.Service.Controllers;

namespace UsbDataTransmitter.Service.StateMachineTypes
{
    public class StateMachine
    {
        private StateMachine<States, Events> _fsm;
        private readonly ISchellenbergService _schellenbergService;
        private readonly ILogger<SchellenbergController> _logger;

        public StateMachine(ISchellenbergService schellenbergService, ILogger<SchellenbergController> logger)
        {
            _schellenbergService = schellenbergService;
            _logger = logger;

            //State Machine 
            ConfiugureStateMachine();
            _fsm.Fire(Events.Init);

            schellenbergService.PairingMessageReceived += SchellenbergService_PairingMessageReceived;
            schellenbergService.StopMessageReceived += SchellenbergService_StopMessageReceived;
            schellenbergService.UpMessageReceived += SchellenbergService_UpMessageReceived;
            schellenbergService.DownMessageReceived += SchellenbergService_DownMessageReceived;

            _logger.LogInformation("FSM startet. Current State: " + _fsm.State);
        }
        
        private void ConfiugureStateMachine()
        {
            _fsm = new StateMachine<States, Events>(States.Unknown);

            _fsm.Configure(States.Unknown).Permit(Events.Init, States.Starting)
                .OnEntryAsync(_schellenbergService.InitStick);

            _fsm.Configure(States.Starting).Permit(Events.Started, States.Idle);

            _fsm.Configure(States.Idle).Permit(Events.MoveUpReceived, States.Moving)
                .OnEntry(_schellenbergService.Up);
            _fsm.Configure(States.Idle).Permit(Events.MoveDownReceived, States.Moving)
                .OnEntry(_schellenbergService.Down);
            _fsm.Configure(States.Idle).Permit(Events.MoveUpPressed, States.Moving)
                .OnEntry(_schellenbergService.Up); ;
            _fsm.Configure(States.Idle).Permit(Events.MoveDownPressed, States.Moving)
                .OnEntry(_schellenbergService.Down);

            _fsm.Configure(States.Idle).Permit(Events.PairingStartedReceived, States.Pairing)
                .OnEntry(_schellenbergService.Pair); 

            _fsm.Configure(States.Moving).Permit(Events.StopReceived, States.Idle);
            _fsm.Configure(States.Moving).Permit(Events.StopPressed, States.Idle);

            _fsm.Configure(States.Pairing).Permit(Events.Paired, States.Idle);

            var graph = MermaidGraph.Format(_fsm.GetInfo());
        }

        public void FireEvent(Events eventToFire)
        {
            _logger.LogInformation($"Current State: {_fsm.State} Event: {eventToFire}");
            _fsm?.Fire(eventToFire);
        }

        private void SchellenbergService_DownMessageReceived(object? sender, SchellenbergEventArgs e)
        {
            FireEvent(Events.MoveDownPressed);
        }

        private void SchellenbergService_UpMessageReceived(object? sender, SchellenbergEventArgs e)
        {
            FireEvent(Events.MoveUpPressed);
        }

        private void SchellenbergService_StopMessageReceived(object? sender, SchellenbergEventArgs e)
        {
            FireEvent(Events.StopPressed);
        }

        private void SchellenbergService_PairingMessageReceived(object? sender, SchellenbergEventArgs e)
        {
            

            FireEvent(Events.PairingStartedReceived);
        }
    }
}
