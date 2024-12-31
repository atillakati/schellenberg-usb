using Stateless;
using UsbDataTransmitter.Service.Controllers;
using UsbDataTransmitter.Service.StateMachineTypes;

namespace UsbDataTransmitter.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<ISchellenbergService, DummyService>();
            builder.Services.AddSingleton(typeof(StateMachine));

            builder.Services.AddControllers();

            var app = builder.Build();           

            // Configure the HTTP request pipeline.            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
