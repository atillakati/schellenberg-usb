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
        Init,

        /// <summary>
        /// Wird ausgelöst, um den Rolladen auf eine prozentuale Position zu fahren.
        /// Diese Implementierung wird im Service durch eine Timer‑Logik verarbeitet.
        /// </summary>
            MoveToPercentReceived
    }

}
