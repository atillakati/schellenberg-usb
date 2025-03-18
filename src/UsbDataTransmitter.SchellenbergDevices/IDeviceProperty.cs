namespace UsbDataTransmitter.SchellenbergDevices
{
    public interface IDeviceProperty
    {
        int Command { get; }
        bool IsActive { get; set; }
        DateTime LastChange { get; }
        string Name { get; }
    }
}