namespace UsbDataTransmitter.Service.Controllers
{
    public interface ISchellenbergService
    {
        string Info { get; }
        string DeviceName { get; }

        void Down();
        void Up();
        void Pair();
        Task InitStick();

        event EventHandler<SchellenbergEventArgs> UpMessageReceived;
        event EventHandler<SchellenbergEventArgs> DownMessageReceived;
        event EventHandler<SchellenbergEventArgs> StopMessageReceived;
        event EventHandler<SchellenbergEventArgs> PairingMessageReceived;
    }
}