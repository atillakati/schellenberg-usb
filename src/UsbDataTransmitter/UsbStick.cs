using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using UsbDataTransmitter.Common;

namespace UsbDataTransmitter;

public class UsbStick : IDisposable
{
    private readonly Action<string, MessageType> _logAction;
    private const int _VID = 0x16C0;
    private const int _PID = 0x05E1;
    private UsbDevice _device;
    private UsbEndpointReader _reader;
    private UsbEndpointWriter _writer;

    public UsbStick(Action<string, MessageType> logAction)
    {
        _logAction = logAction;

        _reader = null;
        _writer = null;
        _device = null;

        Init();
    }

    public static int Vid => _VID;

    public static int Pid => _PID;

    public UsbDevice Device => _device;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~UsbStick()
    {
        Dispose(false);
    }

    public event EventHandler<EndpointDataEventArgs> DataReceived;


    protected void Init()
    {
        var deviceList = UsbDevice.AllLibUsbDevices;
        var usbRegistry = deviceList.Find(x => x.Vid == _VID && x.Pid == _PID);
        if (usbRegistry == null)
        {
            _logAction("Device Not Found.", MessageType.General);
            return;
        }

        usbRegistry.Open(out _device);

        // If this is a "whole" usb device (libusb-win32, linux libusb-1.0) it exposes an IUsbDevice interface. If not (WinUSB) the 
        // 'wholeUsbDevice' variable will be null indicating this is an interface of a device; it does not require or support 
        // configuration and interface selection.
        var wholeUsbDevice = _device as IUsbDevice;
        if (wholeUsbDevice is not null)
        {
            // This is a "whole" USB device. Before it can be used, 
            // the desired configuration and interface must be selected.

            // Select config #1
            wholeUsbDevice.SetConfiguration(1);

            // Claim interface #1.
            wholeUsbDevice.ClaimInterface(1);

            //ShowDeviceInfo(wholeUsbDevice);
        }

        // open read endpoint 1.
        _reader = _device.OpenEndpointReader(ReadEndpointID.Ep01, 128);

        // open write endpoint 1.
        _writer = _device.OpenEndpointWriter(WriteEndpointID.Ep01);
    }

    public int Write(string data)
    {
        if (!_reader.DataReceivedEnabled)
        {
            _reader.DataReceived += _reader_DataReceived;
            _reader.DataReceivedEnabled = true;
        }

        var sendMessage = Encoding.ASCII.GetBytes(data.ToUpper() + "\r\n");
        var result = _writer.Write(sendMessage, 250, out var bytesWritten);

        if (result != ErrorCode.Success)
        {
            _logAction($"ERROR: {result}\n{UsbDevice.LastErrorString}", MessageType.General);
        }
        else
        {
            _logAction(data.ToUpper(), MessageType.Send);
        }

        var lastDataEventDate = DateTime.Now;
        while ((DateTime.Now - lastDataEventDate).TotalMilliseconds < 500)
        {  }

        // Always disable and unhook event when done.
        //_reader.DataReceivedEnabled = false;
        //_reader.DataReceived -= _reader_DataReceived;

        return bytesWritten;
    }

    public string DeviceInfo => _device?.Info.ToString() ?? string.Empty;

    private void _reader_DataReceived(object? sender, EndpointDataEventArgs e)
    {
        OnDataReceived(e);
    }

    protected virtual void OnDataReceived(EndpointDataEventArgs e)
    {
        DataReceived?.Invoke(this, e);
    }

    private void ReleaseUnmanagedResources()
    {
        if (_device != null)
        {
            if (_device.IsOpen)
            {
                _reader.DataReceivedEnabled = false;
                _reader.DataReceived -= _reader_DataReceived;

                // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                // it exposes an IUsbDevice interface. If not (WinUSB) the 
                // 'wholeUsbDevice' variable will be null indicating this is 
                // an interface of a device; it does not require or support 
                // configuration and interface selection.
                var wholeUsbDevice = _device as IUsbDevice;

                // Release interface #0.
                wholeUsbDevice?.ReleaseInterface(1);

                _device.Close();
            }

            // Free usb resources
            UsbDevice.Exit();
        }
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            _reader?.Dispose();
            _writer?.Dispose();
            ((IDisposable)_device)?.Dispose();
        }
    }
}
