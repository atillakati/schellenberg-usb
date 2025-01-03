namespace UsbDataTransmitter.SchellenbergDevices
{
    public class Device : IDevice
    {
        private string _id;
        private int _deviceEnum;
        private string _name;
        private List<IDeviceProperty> _properties;

        public Device(string id, int deviceEnum, string name)
        {
            _deviceEnum = deviceEnum;
            _id = id;
            _name = name;

            _properties = new List<IDeviceProperty>();
        }

        /// <summary>
        /// DeviceID - Unique per remote / Stick / Gateway (eg: 0x265508 )
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// The ID used by the controlling device to identify the paired device.(eg: 0xA1
        /// </summary>
        public int DeviceEnum => _deviceEnum;

        /// <summary>
        /// Custom name of device to identify on UI
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// All available properties like state, position...
        /// </summary>
        public IEnumerable<IDeviceProperty> Properties => _properties;

        /// <summary>
        /// Parses the received data bytes and updates related properties
        /// </summary>
        /// <param name="receivedRawData">eg: ss 0E 265508 1F82 01 00CF</param>
        /// <returns></returns>
        public bool UpdateProperty(string receivedRawData)
        {
            if (string.IsNullOrEmpty(receivedRawData) || receivedRawData.Length != 22)
            {
                return false;
            }

            //string is from right device, check device id
            var id = receivedRawData.Substring(4, 6);
            if (id != _id)
            {
                return false;
            }

            //parse command
            var command = receivedRawData.Substring(14, 2);
            if (string.IsNullOrEmpty(command))
            {
                return false;
            }

            if (command == "00") //=stop
            {
                //set inactive all props
                _properties.ForEach(p => p.IsActive = false);
                return true;
            }

            //update property
            var prop = _properties.FirstOrDefault(p => p.Command == int.Parse(command));
            if (prop != null)
            {
                prop.IsActive = true;
            }

            return true;
        }

        public void AddProperty(DeviceProperty property)
        {
            if (property == null)
            {
                return;
            }

            var existingProperty = _properties.FirstOrDefault(p => p.Name == property.Name);
            if (existingProperty == null)
            {
                _properties.Add(property);
            }
        }

        public string CreateCommandString(IDeviceProperty prop)
        {
            if (prop == null)
            {
                return string.Empty;
            }

            return $"ss{_deviceEnum.ToString("X")}9{prop.Command.ToString("00")}0000";
        }
    }
}
