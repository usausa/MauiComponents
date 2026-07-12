namespace MauiComponents;

using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

internal sealed class FileLoggerWriter : IDisposable
{
    private readonly record struct LogEntry(DateTime Timestamp, string Message);

    private readonly string directory;

    private readonly string prefix;

    private readonly int retainDays;

    private readonly LogFormat format;

    private readonly Channel<LogEntry> channel;

    private readonly Task worker;

    private StreamWriter? writer;

    private DateTime lastDate = DateTime.MinValue.Date;

    public FileLoggerWriter(string directory, string prefix, int retainDays, LogFormat format)
    {
        this.directory = directory;
        this.prefix = prefix;
        this.retainDays = retainDays;
        this.format = format;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        channel = Channel.CreateUnbounded<LogEntry>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
        worker = Task.Run(ProcessAsync);
    }

    public void Dispose()
    {
        channel.Writer.Complete();
        try
        {
            worker.Wait();
        }
        catch (AggregateException)
        {
            // Ignore
        }

        writer?.Dispose();
        writer = null;
    }

    public void Write<TState>(LogLevel logLevel, string categoryName, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var timestamp = DateTime.Now;
        var message = format.Format(logLevel, timestamp, categoryName, state, exception, formatter);
        channel.Writer.TryWrite(new LogEntry(timestamp, message));
    }

    private async Task ProcessAsync()
    {
        var reader = channel.Reader;
        while (await reader.WaitToReadAsync().ConfigureAwait(false))
        {
#pragma warning disable CA1031
            try
            {
                while (reader.TryRead(out var entry))
                {
                    WriteEntry(entry);
                }

                if (writer is not null)
                {
                    await writer.FlushAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
#pragma warning restore CA1031
        }
    }

    private void WriteEntry(LogEntry entry)
    {
        var date = entry.Timestamp.Date;
        if ((writer is null) || (lastDate < date))
        {
            writer?.Dispose();
            writer = null;

            writer = CreateWriter(date);
            lastDate = date;

            DeleteOldFiles(date.AddDays(-retainDays));
        }

        writer!.WriteLine(entry.Message);
    }

#pragma warning disable CA1031
    private void DeleteOldFiles(DateTime date)
    {
        try
        {
            var noPrefix = String.IsNullOrEmpty(prefix);
            var baseFilename = MakeFilename(date);

            foreach (var file in Directory.EnumerateFiles(directory, $"{prefix}*.log"))
            {
                var fi = new FileInfo(file);

                if (fi.Name.EndsWith(".log", StringComparison.Ordinal) &&
                    (noPrefix || fi.Name.StartsWith(prefix, StringComparison.Ordinal)) &&
                    (fi.Name.Length == (12 + prefix.Length)) &&
                    String.CompareOrdinal(fi.Name, baseFilename) <= 0)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
#pragma warning restore CA1031

    private StreamWriter CreateWriter(DateTime date)
    {
        var filename = Path.Combine(directory, MakeFilename(date));
        var fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.Asynchronous);
        fileStream.Seek(0, SeekOrigin.End);
        return new StreamWriter(fileStream);
    }

    private string MakeFilename(DateTime date)
    {
        var builder = new StringBuilder();
        if (!String.IsNullOrEmpty(prefix))
        {
            builder.Append(prefix);
        }

        builder.Append(date.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        builder.Append(".log");

        return builder.ToString();
    }
}
