using System.Text;
using LibUsbDotNet.Main;

namespace UsbDataTransmitter
{
    internal class Programm
    {
        private static UsbStick usbStick;

        public static void Main()
        {
            Console.WriteLine("Attach usb stick with libusb0 v1.4.0.0 driver on windows.\n");

            usbStick = new UsbStick();
            usbStick.DataReceived += Reader_DataReceived;

            Console.WriteLine(usbStick.DeviceInfo);

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

        private static bool isPaired = false;

        private static void Reader_DataReceived(object? sender, EndpointDataEventArgs e)
        {
            var receivedData = Encoding.ASCII.GetString(e.Buffer, 0, e.Count);
        
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] <- " + receivedData);


            if (receivedData.StartsWith("sl") && !isPaired)
            {
                var bytesWritten = usbStick.Write("ssA19400000");
                if (bytesWritten > 0)
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] -> " + "ssA19400000");
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
    }
}