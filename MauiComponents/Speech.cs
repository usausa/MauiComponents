namespace MauiComponents;

using System.Globalization;
using System.Threading.Tasks;

public sealed class SpeechRecognizeEventArgs : EventArgs
{
    public bool Complete { get; }

    public string Text { get; }

    public SpeechRecognizeEventArgs(bool complete, string text)
    {
        Complete = complete;
        Text = text;
    }
}

public interface ISpeechService
{
    event EventHandler<SpeechRecognizeEventArgs>? Recognized;

    // Text to speech

    ValueTask SpeakAsync(string text, float? pitch = null, float? volume = null);

    void SpeakCancel();

    // Speech to text

    ValueTask<bool> RecognizeAsync(CultureInfo cultureInfo);

    ValueTask RecognizeStopAsync();

    void RecognizeCancel();
}
