using System.Text;
using LibUsbDotNet.Main;

namespace UsbDataTransmitter
{
    internal class Programm
    {
        public static void Main()
        {
            Console.WriteLine("Attach usb stick with libusb0 v1.4.0.0 driver on windows.\n");

            var usbStick = new UsbStick();
            usbStick.DataReceived += Reader_DataReceived;

            Console.WriteLine(usbStick.DeviceInfo);

            while (true)
            {
                Console.Write("Enter command: ");
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

        private static void Reader_DataReceived(object? sender, EndpointDataEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] " + Encoding.ASCII.GetString(e.Buffer, 0, e.Count));
        }
    }
}