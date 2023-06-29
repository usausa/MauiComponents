namespace MauiComponents;

using Microsoft.Extensions.Logging;

public sealed class MessageLogFormat : LogFormat
{
    public static MessageLogFormat Instance { get; } = new();

    public override string Format<TState>(
        LogLevel logLevel,
        DateTime timestamp,
        string categoryName,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) => formatter(state, exception);
}
