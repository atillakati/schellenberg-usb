
namespace UsbDataTransmitter.SchellenbergDevices
{
    public interface IDevice
    {
        int DeviceEnum { get; }
        string Id { get; }
        string Name { get; }
        IEnumerable<IDeviceProperty> Properties { get; }

        void AddProperty(DeviceProperty property);
        bool UpdateProperty(string receivedRawData);

        string CreateCommandString(IDeviceProperty prop);
    }
}