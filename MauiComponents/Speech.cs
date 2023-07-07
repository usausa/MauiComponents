namespace MauiComponents;

using System.Globalization;

public interface ISpeechService
{
    // Text to speech

    ValueTask SpeakAsync(string text, float? pitch = null, float? volume = null);

    void SpeakCancel();

    // Speech to text

    ValueTask<string?> RecognizeAsync(Action<string> progress, CultureInfo? culture = null, CancellationToken cancel = default);
}
