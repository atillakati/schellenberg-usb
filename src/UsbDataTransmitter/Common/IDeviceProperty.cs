
namespace UsbDataTransmitter.Common
{
    public interface IDeviceProperty
    {
        int Command { get; }
        bool IsActive { get; set; }
        DateTime LastChange { get; }
        string Name { get; }
    }
}