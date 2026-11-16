namespace MauiComponents;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class AndroidLoggerProvider : ILoggerProvider
{
    private readonly AndroidLoggerOptions options;

    public AndroidLoggerProvider(IOptions<AndroidLoggerOptions> options)
    {
        this.options = options.Value;
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (options.ShortCategory)
        {
#if NET11_0_OR_GREATER
            var index = categoryName.LastIndexOf('.', StringComparison.Ordinal);
#else
            var index = categoryName.LastIndexOf('.');
#endif
            if (index >= 0)
            {
                categoryName = categoryName[(index + 1)..];
            }
        }

        return new AndroidLogger(categoryName, options.Threshold, options.Format ?? MessageLogFormat.Instance);
    }
}
