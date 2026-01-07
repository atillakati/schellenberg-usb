namespace UsbDataTransmitter.Service.Entities
{
    public class DeviceInfo
    {
        public string version { get; set; }
        public DateTime lastUpdate { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public string fsm_state { get; set; }
            public int position { get; set; } // 0 = oben (fully open), 100 = unten (fully closed)
    }
}

