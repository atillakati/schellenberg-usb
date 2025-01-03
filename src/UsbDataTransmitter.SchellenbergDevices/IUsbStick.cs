﻿namespace UsbDataTransmitter.SchellenbergDevices
{
    public interface IUsbStick : IDisposable
    {        
        string DeviceInfo { get; }

        event EventHandler<UsbDataReceivedEventArgs> DataReceived; 
        
        int Write(string data);
    }
}