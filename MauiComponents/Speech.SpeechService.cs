namespace MauiComponents;

using System.Globalization;

using CommunityToolkit.Maui.Media;

public sealed class SpeechService : ISpeechService, IDisposable
{
    private readonly ITextToSpeech textToSpeech;

    private readonly ISpeechToText speechToText;

    private CancellationTokenSource? cts;

    public SpeechService(
        ITextToSpeech textToSpeech,
        ISpeechToText speechToText)
    {
        this.textToSpeech = textToSpeech;
        this.speechToText = speechToText;
    }

    public void Dispose()
    {
        cts?.Dispose();
    }

    // ------------------------------------------------------------
    // Text to speech
    // ------------------------------------------------------------

    public async ValueTask SpeakAsync(string text, float? pitch, float? volume)
    {
        cts = new CancellationTokenSource();
        var options = new SpeechOptions
        {
            Pitch = pitch,
            Volume = volume
        };
        await textToSpeech.SpeakAsync(text, options, cts.Token).ConfigureAwait(true);
    }

    public void SpeakCancel()
    {
        if (cts?.IsCancellationRequested ?? true)
        {
            return;
        }

        cts.Cancel();
    }

    // ------------------------------------------------------------
    // Speech to text
    // ------------------------------------------------------------

    public async ValueTask<string?> RecognizeAsync(Progress<string> progress, CultureInfo? culture, CancellationToken cancel)
    {
        var granted = await speechToText.RequestPermissions().ConfigureAwait(true);
        if (!granted)
        {
            return null;
        }

#pragma warning disable CA1031
        try
        {
            var result = await speechToText.ListenAsync(
                culture ?? CultureInfo.CurrentCulture,
                progress,
                cancel).ConfigureAwait(true);
            result.EnsureSuccess();

            if (result.IsSuccessful)
            {
                return result.Text;
            }
        }
        catch
        {
            // Ignore
        }
#pragma warning restore CA1031

        return null;
    }
}
