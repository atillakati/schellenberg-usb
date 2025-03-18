namespace UsbDataTransmitter.Service.StateMachineTypes
{
    public enum States
    {
        Starting,
        Idle,
        Closed,
        Closing,
        Opening,
        Open,
        Pairing,
        Unknown
    }
}