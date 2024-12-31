namespace UsbDataTransmitter.Service.Controllers
{
    public interface ISchellenbergService
    {
        string Info { get; }
        string DeviceName { get; }

        void Down();
        void Up();
        void Pair();
        void InitStick();

        event EventHandler<SchellenbergEventArgs> EventReceived;        
        event EventHandler<SchellenbergEventArgs> PairingMessageReceived;
    }
}