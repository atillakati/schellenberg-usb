using System.Text;
using LibUsbDotNet.Main;
using UsbDataTransmitter.Common;

namespace UsbDataTransmitter
{
    internal class Programm
    {
        private static UsbStick usbStick;
        private static bool isPaired = false;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("\r\n _   _     _    ______      _      _____                             _ _   _             \r\n| | | |   | |   |  _  \\    | |    |_   _|                           (_) | | |            \r\n| | | |___| |__ | | | |__ _| |_ __ _| |_ __ __ _ _ __  ___ _ __ ___  _| |_| |_ ___ _ __  \r\n| | | / __| '_ \\| | | / _` | __/ _` | | '__/ _` | '_ \\/ __| '_ ` _ \\| | __| __/ _ \\ '__| \r\n| |_| \\__ \\ |_) | |/ / (_| | || (_| | | | | (_| | | | \\__ \\ | | | | | | |_| ||  __/ |    \r\n \\___/|___/_.__/|___/ \\__,_|\\__\\__,_\\_/_|  \\__,_|_| |_|___/_| |_| |_|_|\\__|\\__\\___|_|    \r\n                                                                                         \r\n                                                                                         ");
            Console.WriteLine("Attach usb stick with libusb0 v1.4.0.0 driver on windows.");
            Console.WriteLine("Usage:\nUsbDataTransmitter [-autoInit]");
            Console.WriteLine();

            usbStick = new UsbStick(LogMessage);
            usbStick.DataReceived += Reader_DataReceived;

            Console.WriteLine(usbStick.DeviceInfo);

            await ExecuteCommandOptions(args);

            while (true)
            {
                Console.WriteLine("Enter command: ");
                var cmdLine = Console.ReadLine()?.ToUpper();

                if (!string.IsNullOrEmpty(cmdLine))
                {
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
            
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss,fff")}] {msgType}" + message);
        }

        private static void Reader_DataReceived(object? sender, EndpointDataEventArgs e)
        {
            var receivedData = Encoding.ASCII.GetString(e.Buffer, 0, e.Count);
            LogMessage(receivedData, MessageType.Receive);
            
            if (receivedData.StartsWith("sl") && !isPaired)
            {
                var bytesWritten = usbStick.Write("ssA19400000");
                if (bytesWritten > 0)
                {
                    isPaired = true;
                }
            }
            //else if (receivedData.StartsWith("t0"))
            //{
            //    var bytesWritten = usbStick.Write("ssA19000000");
            //    if (bytesWritten > 0)
            //    {
            //        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] -> " + "ssA19000000");
            //    }
            //}
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

            usbStick.Write("!g");
            Thread.Sleep(200);
            usbStick.Write("!?");
            Thread.Sleep(200);
            usbStick.Write("hello");
            Thread.Sleep(200);
            usbStick.Write("!?");
        }
    }
}