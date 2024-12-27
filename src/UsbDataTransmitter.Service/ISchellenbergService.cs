namespace UsbDataTransmitter.Service.Controllers
{
    public interface ISchellenbergService
    {
        string Info { get; }
        string DeviceName { get; }

        void Down();
        void Up();
    }
}