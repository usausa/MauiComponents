namespace MauiComponents;

using Microsoft.Extensions.Logging;

internal sealed class FileLogger : ILogger
{
    private readonly string categoryName;

    private readonly LogLevel threshold;

    private readonly FileLoggerWriter writer;

    public FileLogger(string categoryName, LogLevel threshold, FileLoggerWriter writer)
    {
        this.categoryName = categoryName;
        this.threshold = threshold;
        this.writer = writer;
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= threshold;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
        => NullScope.Instance;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        writer.Write(logLevel, categoryName, state, exception, formatter);
    }
}
