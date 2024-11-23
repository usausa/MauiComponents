namespace MauiComponents;

using System.Diagnostics;
using System.Globalization;
using System.Text;

using Microsoft.Extensions.Logging;

internal sealed class FileLoggerWriter : IDisposable
{
    private readonly Lock sync = new();

    private readonly string directory;

    private readonly string prefix;

    private readonly int retainDays;

    private readonly LogFormat format;

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
    }

    public void Dispose()
    {
        writer?.Dispose();
    }

    public void Write<TState>(LogLevel logLevel, string categoryName, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (sync)
        {
            var timestamp = DateTime.Now;
            var message = format.Format(logLevel, timestamp, categoryName, state, exception, formatter);

            var date = timestamp.Date;
            if ((lastDate < date) || (writer is null))
            {
                writer?.Dispose();
                writer = CreateWriter(date);

                lastDate = date;

                Task.Run(() => DeleteOldFiles(date.AddDays(-retainDays)));
            }

            writer!.WriteLine(message);
            writer.Flush();
        }
    }

    private void DeleteOldFiles(DateTime date)
    {
        var noPrefix = String.IsNullOrEmpty(prefix);
        var baseFilename = MakeFilename(date);

        foreach (var file in Directory.GetFiles(directory))
        {
            var fi = new FileInfo(file);

            if (fi.Name.EndsWith(".log", StringComparison.Ordinal) &&
                (noPrefix || fi.Name.StartsWith(prefix!, StringComparison.Ordinal)) &&
                (fi.Name.Length == (12 + prefix?.Length)) &&
                String.CompareOrdinal(fi.Name, baseFilename) <= 0)
            {
#pragma warning disable CA1031
                try
                {
                    File.Delete(Path.Combine(directory, file));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
#pragma warning restore CA1031
            }
        }
    }

    private StreamWriter CreateWriter(DateTime date)
    {
        var filename = Path.Combine(directory, MakeFilename(date));
        var fileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
