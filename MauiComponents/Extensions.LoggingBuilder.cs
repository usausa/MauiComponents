namespace MauiComponents;

using Microsoft.Extensions.Logging;

public static class LoggingBuilderExtensions
{
#if ANDROID
    public static ILoggingBuilder AddAndroidLogger(this ILoggingBuilder builder)
    {
        return builder.AddAndroidLogger(static _ => { });
    }

    public static ILoggingBuilder AddAndroidLogger(this ILoggingBuilder builder, Action<AndroidLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, AndroidLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
#endif

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder)
    {
        return builder.AddFileLogger(static _ => { });
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}
