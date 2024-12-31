namespace UsbDataTransmitter.Service.Controllers
{
    public class SchellenbergEventArgs : EventArgs
    {                
        public string RawMessage { get; set; }

        public bool Paired { get; set; }
    }
}