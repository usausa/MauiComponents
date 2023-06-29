namespace MauiComponents;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly FileLoggerOptions options;

    private readonly FileLoggerWriter writer;

    public FileLoggerProvider(IOptions<FileLoggerOptions> options)
    {
        this.options = options.Value;
        writer = new FileLoggerWriter(
            options.Value.Directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Log"),
            options.Value.Prefix ?? string.Empty,
            options.Value.RetainDays,
            options.Value.Format ?? SimpleLogFormat.Instance);
    }

    public void Dispose()
    {
        writer.Dispose();
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (options.ShortCategory)
        {
            var index = categoryName.LastIndexOf('.');
            if (index >= 0)
            {
                categoryName = categoryName[(index + 1)..];
            }
        }

        return new FileLogger(categoryName, options.Threshold, writer);
    }
}
