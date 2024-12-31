using Microsoft.Extensions.Logging.Console;
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
            builder.Services.AddSingleton<IStateMachine, StateMachine>();

            builder.Services.AddControllers();

            builder.Logging.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss,fff ";
                options.SingleLine = true;
                options.IncludeScopes = false;                
            });


            var app = builder.Build();           

            // Configure the HTTP request pipeline.            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
