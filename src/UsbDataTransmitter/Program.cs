using System.Reflection;
using System.Text;
using log4net.Config;
using log4net;
using UsbDataTransmitter.SchellenbergDevices;

namespace UsbDataTransmitter
{
    internal class Programm
    {
        private static IUsbStick usbStick;
        private static bool isPaired = false;
        private static IDevice device;
        private static ILog _logger;// = log4net.LogManager.GetLogger(typeof(Programm));

        public static async Task Main(string[] args)
        {
            Console.WriteLine("\r\n _   _     _    ______      _      _____                             _ _   _             \r\n| | | |   | |   |  _  \\    | |    |_   _|                           (_) | | |            \r\n| | | |___| |__ | | | |__ _| |_ __ _| |_ __ __ _ _ __  ___ _ __ ___  _| |_| |_ ___ _ __  \r\n| | | / __| '_ \\| | | / _` | __/ _` | | '__/ _` | '_ \\/ __| '_ ` _ \\| | __| __/ _ \\ '__| \r\n| |_| \\__ \\ |_) | |/ / (_| | || (_| | | | | (_| | | | \\__ \\ | | | | | | |_| ||  __/ |    \r\n \\___/|___/_.__/|___/ \\__,_|\\__\\__,_\\_/_|  \\__,_|_| |_|___/_| |_| |_|_|\\__|\\__\\___|_|    \r\n                                                                                         \r\n                                                                                         ");
            Console.WriteLine("Attach usb stick with libusb0 v1.4.0.0 driver on windows.");
            Console.WriteLine("Usage:\nUsbDataTransmitter [-autoInit]");
            Console.WriteLine();

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            _logger = LogManager.GetLogger(typeof(Programm));

            usbStick = new UsbStick(LogMessage);
            usbStick.DataReceived += Reader_DataReceived;

            //create device instance
            device = new Device("265508", 0xA1, "Schellenberg Rollodrive Premium");
            device.AddProperty(new DeviceProperty("up", 0x01));
            device.AddProperty(new DeviceProperty("down", 0x02));

            Console.WriteLine(usbStick.DeviceInfo);

            await ExecuteCommandOptions(args);            

            _logger.Info("Application startet...");

            while (true)
            {
                Console.WriteLine("Enter command: ");
                var cmdLine = Console.ReadLine();

                if (!string.IsNullOrEmpty(cmdLine))
                {
                    var prop = device.Properties.FirstOrDefault(p => p.Name == cmdLine);
                    if (prop != null)
                    {
                        cmdLine = device.CreateCommandString(prop);
                    }

                    var bytesWritten = usbStick.Write(cmdLine);
                    Console.WriteLine("Done! ({0} bytes)", bytesWritten);
                }
                else
                {
                    break;
                }
            }

            usbStick.DataReceived -= Reader_DataReceived;
            usbStick.Dispose();
        }


        private static void LogMessage(string message, MessageType type)
        {
            var msgType = string.Empty;

            switch (type)
            {
                case MessageType.Send:
                    msgType = "-> ";
                    break;
                case MessageType.Receive:
                    msgType = "<- ";
                    break;

                default:
                    msgType = string.Empty;
                    break;
            }
                        
            _logger.Info($" {msgType}" + message);
        }

        private static void Reader_DataReceived(object? sender, UsbDataReceivedEventArgs e)
        {
            if(!e.Count.HasValue || e.Buffer == null)
            {
                return;
            }

            var receivedData = Encoding.ASCII.GetString(e.Buffer, 0, e.Count.Value);
            LogMessage(receivedData, MessageType.Receive);

            //pairing
            if (receivedData.StartsWith("sl") && !isPaired)
            {
                var bytesWritten = usbStick.Write("ssA19600000");
                if (bytesWritten > 0)
                {
                    isPaired = true;
                }
            }
            else
            {
                device.UpdateProperty(receivedData);
            }
        }

        private static async Task ExecuteCommandOptions(string[] args)
        {
            if (args.Length > 0)
            {
                //get command
                switch (args[0])
                {
                    case "-autoInit":
                        await InitStick();
                        break;
                }
            }
        }

        private static async Task InitStick()
        {
            LogMessage("Initializing RF stick...", MessageType.General);

            usbStick.Write("!G");
            Thread.Sleep(200);
            usbStick.Write("!?");
            Thread.Sleep(200);
            usbStick.Write("hello");
            Thread.Sleep(200);
            usbStick.Write("!?");
        }
    }
}