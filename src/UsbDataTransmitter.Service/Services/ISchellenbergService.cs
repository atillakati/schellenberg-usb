using UsbDataTransmitter.Service.StateMachineTypes;
using System.Threading.Tasks;

namespace UsbDataTransmitter.Service.Services
{
    public interface ISchellenbergService
    {
        string Info { get; }
        string DeviceName { get; }

        States CurrentFsmState { get; }
                int CurrentPosition { get; }
        Task MoveToAsync(int percent);

      
        void FireEvent(Events eventToFire);

        //void Close();
        //void Init();
    }

}
