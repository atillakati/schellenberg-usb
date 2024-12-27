using LibUsbDotNet.Main;

namespace UsbDataTransmitter.SchellenbergDevices
{
    public class UsbDataReceivedEventArgs : EventArgs
    {
        private readonly EndpointDataEventArgs _endpointDataEventArgs;

        public UsbDataReceivedEventArgs(EndpointDataEventArgs args)
        {
            _endpointDataEventArgs = args;            
        }

        public byte[]? Buffer => _endpointDataEventArgs?.Buffer;

        public int? Count => _endpointDataEventArgs?.Count;
    }
}