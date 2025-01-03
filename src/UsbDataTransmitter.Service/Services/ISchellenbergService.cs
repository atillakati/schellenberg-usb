using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service.Services
{
    public interface ISchellenbergService
    {
        string Info { get; }
        string DeviceName { get; }

        States CurrentFsmState { get; }

        void FireEvent(Events eventToFire);
    }
}