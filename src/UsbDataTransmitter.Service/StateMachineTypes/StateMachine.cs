using Microsoft.Extensions.Logging;
using Stateless;
using Stateless.Graph;
using UsbDataTransmitter.Service.Controllers;

namespace UsbDataTransmitter.Service.StateMachineTypes
{
    public class StateMachine : IStateMachine
    {
        private StateMachine<States, Events> _fsm;
        private readonly ISchellenbergService _schellenbergService;
        private readonly ILogger<StateMachine> _logger;
        

        public StateMachine(ISchellenbergService schellenbergService, ILogger<StateMachine> logger)
        {
            _schellenbergService = schellenbergService;
            _logger = logger;

            //State Machine 
            SetupStateMachine();

            schellenbergService.EventReceived += SchellenbergService_EventReceived;  
            schellenbergService.PairingMessageReceived += SchellenbergService_PairingMessageReceived;

            _fsm.Activate();
            _logger.LogInformation("FSM startet. Current State: " + _fsm.State);            
        }
        
        private void SetupStateMachine()
        {
            _fsm = new StateMachine<States, Events>(States.Unknown);
            _fsm.OnUnhandledTrigger((s,e) => _logger.LogWarning($"Unhandled trigger - State: {s} Event: {e}"));
            _fsm.OnTransitionCompleted((trans) => _logger.LogInformation($"FSM state changed: {trans.Source} -> {trans.Trigger} => {trans.Destination}"));

            _fsm.Configure(States.Unknown).Permit(Events.Init, States.Starting)
                .OnEntry(_schellenbergService.InitStick);

            _fsm.Configure(States.Starting).Permit(Events.Started, States.Idle);

            //_fsm.Configure(States.Idle).Permit(Events.MoveUpReceived, States.Moving)
            //    .OnEntry(_schellenbergService.Up);
            //_fsm.Configure(States.Idle).Permit(Events.MoveUpPressed, States.Moving)
            //    .OnEntry(_schellenbergService.Up);
            //_fsm.Configure(States.Idle).Permit(Events.MoveDownReceived, States.Moving)
            //    .OnEntry(_schellenbergService.Down);            
            //_fsm.Configure(States.Idle).Permit(Events.MoveDownPressed, States.Moving)
            //    .OnEntry(_schellenbergService.Down);

            //_fsm.Configure(States.Idle).Permit(Events.PairingStartedReceived, States.Pairing)
            //    .OnEntry(_schellenbergService.Pair); 

            //_fsm.Configure(States.Moving).Permit(Events.StopReceived, States.Idle);
            //_fsm.Configure(States.Moving).Permit(Events.StopPressed, States.Idle);

            //_fsm.Configure(States.Pairing).Permit(Events.Paired, States.Idle);

            //var graph = MermaidGraph.Format(_fsm.GetInfo());            
        }

        public string CurrentState => _fsm.State.ToString();

        public void FireEvent(Events eventToFire)
        {
            _logger.LogInformation($"FireEvent - Current State: {_fsm.State} Event: {eventToFire}");
            _fsm?.Fire(eventToFire);
        }


        private void SchellenbergService_EventReceived(object? sender, SchellenbergEventArgs e)
        {
            _logger.LogInformation($"SchellenbergService_EventReceived called => {e.CurrentEvent}");

            FireEvent(e.CurrentEvent);
        }

        private void SchellenbergService_PairingMessageReceived(object? sender, SchellenbergEventArgs e)
        {
            if (!e.Paired)
            {
                FireEvent(Events.PairingStartedReceived);
            }
            else
            {
                FireEvent(Events.Paired);
            }
        }
    }
}
