namespace MauiComponents;

using System.Globalization;
using System.Threading;

using CommunityToolkit.Maui.Media;

public sealed class SpeechService : ISpeechService, IDisposable
{
    public event EventHandler<SpeechRecognizeEventArgs>? Recognized;

    private readonly ITextToSpeech textToSpeech;

    private readonly ISpeechToText speechToText;

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
        ctsSpeak = new CancellationTokenSource();
        var options = new SpeechOptions
        {
            Pitch = pitch,
            Volume = volume
        };
        await textToSpeech.SpeakAsync(text, options, ctsSpeak.Token).ConfigureAwait(true);
    }

    public void SpeakCancel()
    {
        if (ctsSpeak?.IsCancellationRequested ?? true)
        {
            return;
        }

        ctsSpeak.Cancel();
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

        ctsRecognize = new CancellationTokenSource();
        var option = new SpeechToTextOptions
        {
            Culture = cultureInfo,
            ShouldReportPartialResults = true
        };
        await speechToText.StartListenAsync(option, ctsRecognize.Token).ConfigureAwait(true);

        return true;
    }

    public async ValueTask RecognizeStopAsync()
    {
        await speechToText.StopListenAsync(CancellationToken.None).ConfigureAwait(true);
    }

    public void RecognizeCancel()
    {
        speechToText.StopListenAsync(CancellationToken.None);

        if (ctsRecognize?.IsCancellationRequested ?? true)
        {
            return;
        }

        ctsRecognize.Cancel();
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
