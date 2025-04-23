using Microsoft.Extensions.Logging;

namespace UsbDataTransmitter.SchellenbergDevices;

public class ConsoleLogger : ILogger<UsbStick>
{
    private readonly Action<string, MessageType> _logAction;

    public ConsoleLogger(Action<string, MessageType> logAction)
    {
        _logAction = logAction;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _logAction(formatter(state, exception), MessageType.General);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}