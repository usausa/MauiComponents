namespace MauiComponents;

using Microsoft.Extensions.Logging;

internal sealed class AndroidLogger : ILogger
{
    private readonly string categoryName;

    private readonly LogLevel threshold;

    private readonly LogFormat format;

    public AndroidLogger(string categoryName, LogLevel threshold, LogFormat format)
    {
        this.categoryName = categoryName;
        this.threshold = threshold;
        this.format = format;
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

        var timestamp = DateTime.Now;
        var message = format.Format(logLevel, timestamp, categoryName, state, exception, formatter);
        var throwable = exception is not null ? Java.Lang.Throwable.FromException(exception) : null;

        switch (logLevel)
        {
            case LogLevel.Trace:
                Android.Util.Log.Verbose(categoryName, throwable!, message);
                break;
            case LogLevel.Debug:
                Android.Util.Log.Debug(categoryName, throwable!, message);
                break;
            case LogLevel.Information:
                Android.Util.Log.Info(categoryName, throwable!, message);
                break;
            case LogLevel.Warning:
                Android.Util.Log.Warn(categoryName, throwable!, message);
                break;
            case LogLevel.Error:
                Android.Util.Log.Error(categoryName, throwable!, message);
                break;
            case LogLevel.Critical:
                Android.Util.Log.Wtf(categoryName, throwable!, message);
                break;
        }
    }
}
