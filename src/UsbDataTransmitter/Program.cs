using System;
using System.Text;
using System.Text.RegularExpressions;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.Info;

namespace Examples
{
    internal class Programm
    {
        public static void Main(string[] args)
        {
            UsbDevice selectedDevice = null;

            //var selectedDevice = UsbDevice.OpenUsbDevice(x => x.Vid == 0x16C0 && x.Pid == 0x5E1);
            var deviceList = UsbDevice.AllLibUsbDevices;
            var usbRegistry = deviceList.Find(x => x.Vid == 0x16C0 && x.Pid == 0x5E1);
            if (usbRegistry == null)
            {
                throw new Exception("Device Not Found.");
            }
            
            usbRegistry.Open(out selectedDevice);

            // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
            // it exposes an IUsbDevice interface. If not (WinUSB) the 
            // 'wholeUsbDevice' variable will be null indicating this is 
            // an interface of a device; it does not require or support 
            // configuration and interface selection.
            IUsbDevice wholeUsbDevice = selectedDevice as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);

                // Claim interface #0.
                wholeUsbDevice.ClaimInterface(1);

                ShowDeviceInfo(wholeUsbDevice);
            }

            // open read endpoint 1.
            UsbEndpointReader reader = selectedDevice.OpenEndpointReader(ReadEndpointID.Ep01, 128);
           
            
            // open write endpoint 1.
            UsbEndpointWriter writer = selectedDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

            while (true)
            {
                Console.Write("Enter command: ");
                string cmdLine = Console.ReadLine().ToUpper();

                if (!string.IsNullOrEmpty(cmdLine))
                {
                    int bytesWritten;

                    reader.DataReceived += Reader_DataReceived;
                    reader.DataReceivedEnabled = true;

                    var ec = writer.Write(Encoding.ASCII.GetBytes(cmdLine + "\r\n"), 2000, out bytesWritten);
                    Console.WriteLine($"{ec} - {bytesWritten} bytes written");
                    if (ec != ErrorCode.Success)
                    {
                        Console.WriteLine("ERROR: " + ec.ToString() + UsbDevice.LastErrorString);
                    }

                    Console.WriteLine("..waiting for answer..");
                    var LastDataEventDate = DateTime.Now;
                    while ((DateTime.Now - LastDataEventDate).TotalMilliseconds < 2000)
                    {
                    }

                    // Always disable and unhook event when done.
                    reader.DataReceivedEnabled = false;
                    reader.DataReceived -= Reader_DataReceived;

                    Console.WriteLine("Done!");
                }
                else
                {
                    break;
                }
            }

            if (selectedDevice != null)
            {
                Console.WriteLine("Free usb resources...");
                if (selectedDevice.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    wholeUsbDevice = selectedDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(1);
                    }

                    selectedDevice.Close();
                }
                selectedDevice = null;

                // Free usb resources
                UsbDevice.Exit();
            }
        }

        private static void Reader_DataReceived(object? sender, EndpointDataEventArgs e)
        {
            Console.WriteLine("Data received: " + Encoding.Default.GetString(e.Buffer, 0, e.Count));
        }

        private static void ShowDeviceInfo(IUsbDevice selectedDevice)
        {
            Console.WriteLine($"{selectedDevice.Info}");
        }
    }
}