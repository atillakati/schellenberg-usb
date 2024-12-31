using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service.Controllers
{
    public class SchellenbergEventArgs : EventArgs
    {                
        public string RawMessage { get; set; }

        public bool Paired { get; set; }

        public Events CurrentEvent { get; set; }
    }
}