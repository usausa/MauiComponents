namespace MauiComponents;

using System.Globalization;
using System.Threading;

using CommunityToolkit.Maui.Media;

public sealed class SpeechService : ISpeechService, IDisposable
{
    public event EventHandler<SpeechRecognizeEventArgs>? Recognized;

    private readonly ITextToSpeech textToSpeech;

    private readonly ISpeechToText speechToText;

    private readonly Lock sync = new();

    private CancellationTokenSource? ctsSpeak;

    private CancellationTokenSource? ctsRecognize;

    public SpeechService(
        ITextToSpeech textToSpeech,
        ISpeechToText speechToText)
    {
        this.textToSpeech = textToSpeech;
        this.speechToText = speechToText;
        speechToText.RecognitionResultUpdated += SpeechToTextOnRecognitionResultUpdated;
        speechToText.RecognitionResultCompleted += SpeechToTextOnRecognitionResultCompleted;
    }

    public void Dispose()
    {
        speechToText.RecognitionResultUpdated -= SpeechToTextOnRecognitionResultUpdated;
        speechToText.RecognitionResultCompleted -= SpeechToTextOnRecognitionResultCompleted;
        ctsSpeak?.Dispose();
        ctsRecognize?.Dispose();
    }

    // ------------------------------------------------------------
    // Text to speech
    // ------------------------------------------------------------

    public async ValueTask SpeakAsync(string text, float? pitch, float? volume)
    {
        var source = new CancellationTokenSource();
        CancellationTokenSource? previous;
        lock (sync)
        {
            previous = ctsSpeak;
            ctsSpeak = source;
        }

        if (previous is not null)
        {
            await previous.CancelAsync().ConfigureAwait(true);
            previous.Dispose();
        }

        var options = new SpeechOptions
        {
            Pitch = pitch,
            Volume = volume
        };
        try
        {
            await textToSpeech.SpeakAsync(text, options, source.Token).ConfigureAwait(true);
        }
        finally
        {
            ReleaseSpeak(source);
        }
    }

    public void SpeakCancel()
    {
        CancellationTokenSource? source;
        lock (sync)
        {
            source = ctsSpeak;
        }

        CancelSource(source);
    }

    // ------------------------------------------------------------
    // Speech to text
    // ------------------------------------------------------------

    public async ValueTask<bool> RecognizeAsync(CultureInfo cultureInfo)
    {
        if (!await speechToText.RequestPermissions(CancellationToken.None).ConfigureAwait(true))
        {
            return false;
        }

        var source = new CancellationTokenSource();
        CancellationTokenSource? previous;
        lock (sync)
        {
            previous = ctsRecognize;
            ctsRecognize = source;
        }

        if (previous is not null)
        {
            await previous.CancelAsync().ConfigureAwait(true);
            previous.Dispose();
        }

        var option = new SpeechToTextOptions
        {
            Culture = cultureInfo,
            ShouldReportPartialResults = true
        };
        await speechToText.StartListenAsync(option, source.Token).ConfigureAwait(true);

        return true;
    }

    public async ValueTask RecognizeStopAsync()
    {
        await speechToText.StopListenAsync(CancellationToken.None).ConfigureAwait(true);
    }

    public void RecognizeCancel()
    {
        CancellationTokenSource? source;
        lock (sync)
        {
            source = ctsRecognize;
        }

        CancelSource(source);
    }

    public async ValueTask RecognizeCancelAsync()
    {
        CancellationTokenSource? source;
        lock (sync)
        {
            source = ctsRecognize;
        }

        if ((source is not null) && !source.IsCancellationRequested)
        {
            try
            {
                await source.CancelAsync().ConfigureAwait(true);
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
        }

        await speechToText.StopListenAsync(CancellationToken.None).ConfigureAwait(true);
    }

    private static void CancelSource(CancellationTokenSource? source)
    {
        if ((source is null) || source.IsCancellationRequested)
        {
            return;
        }

        try
        {
            source.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // Ignore
        }
    }

    private void ReleaseSpeak(CancellationTokenSource source)
    {
        lock (sync)
        {
            if (!ReferenceEquals(ctsSpeak, source))
            {
                return;
            }

            ctsSpeak = null;
        }

        source.Dispose();
    }

    private void SpeechToTextOnRecognitionResultUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
    {
        Recognized?.Invoke(this, new SpeechRecognizeEventArgs(false, e.RecognitionResult));
    }

    private void SpeechToTextOnRecognitionResultCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
    {
        Recognized?.Invoke(this, new SpeechRecognizeEventArgs(true, e.RecognitionResult.Text ?? string.Empty));
    }
}
