namespace MauiComponents;

public sealed class SpeechService : ISpeechService, IDisposable
{
    private readonly ITextToSpeech textToSpeech;

    private CancellationTokenSource? cts;

    public SpeechService(
        ITextToSpeech textToSpeech)
    {
        this.textToSpeech = textToSpeech;
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

    // TODO
}
