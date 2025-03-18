namespace UsbDataTransmitter.SchellenbergDevices
{
    public interface IUsbStick : IDisposable
    {        
        string DeviceInfo { get; }

        event EventHandler<UsbDataReceivedEventArgs> DataReceived;

        //void Init();

        int Write(string data);
    }
}