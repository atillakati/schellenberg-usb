using log4net;

namespace UsbDataTransmitter.SchellenbergDevices
{
    public class DeviceProperty : IDeviceProperty
    {
        private readonly ILog _log;
        private bool _isActive;
        private DateTime _lastChange;
        private string _name;
        private int _command;

        public DeviceProperty(ILog logger, string name, int command)
        {
            _log = logger;

            _isActive = false;
            _lastChange = DateTime.MinValue;
            _name = name;
            _command = command;
        }

        public DateTime LastChange => _lastChange;

        /// <summary>
        /// Name of a property. Must be uniqe.
        /// </summary>
        public string Name => _name;
        public int Command => _command;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                _lastChange = DateTime.Now;
                _log.Info($"{_name} has changed to {IsActive}");
            }
        }
    }
}