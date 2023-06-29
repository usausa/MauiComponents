namespace MauiComponents;

using Microsoft.Extensions.Logging;

public sealed class FileLoggerOptions
{
    public bool ShortCategory { get; set; }

    public LogLevel Threshold { get; set; } = LogLevel.Information;

    public LogFormat? Format { get; set; }

    public string? Directory { get; set; }

    public string? Prefix { get; set; }

    public int RetainDays { get; set; } = 30;
}
