namespace MauiComponents;

using Microsoft.Extensions.Logging;

public abstract class LogFormat
{
    public abstract string Format<TState>(
        LogLevel logLevel,
        DateTime timestamp,
        string categoryName,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter);
}
