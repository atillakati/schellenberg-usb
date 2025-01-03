using UsbDataTransmitter.SchellenbergDevices;
using UsbDataTransmitter.Service.Services;

namespace UsbDataTransmitter.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddSingleton<IUsbStick, UsbStick>();
            builder.Services.AddSingleton<IUsbStick, DummyUsbStickImplementation>();
            builder.Services.AddSingleton<ISchellenbergService, SchellenbergService>();            

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
