namespace UsbDataTransmitter.Service.StateMachineTypes
{
    public interface IStateMachine
    {
        string CurrentState { get; }

        void FireEvent(Events eventToFire);
    }
}