namespace UsbDataTransmitter.Service.StateMachineTypes
{
    public interface IStateMachine
    {
        void FireEvent(Events eventToFire);
    }
}