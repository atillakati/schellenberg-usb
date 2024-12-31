namespace UsbDataTransmitter.Service.StateMachineTypes
{
    public enum Events
    {
        MoveUpReceived,
        StopReceived,
        MoveDownReceived,
        PairingStartedReceived,
        MoveUpPressed,
        MoveDownPressed,
        StopPressed,
        Started,
        Paired,
        Init
    }
}