namespace MauiComponents;

using System.Threading.Tasks;

public interface ISpeechService
{
    // Text to speech

    ValueTask SpeakAsync(string text, float? pitch = null, float? volume = null);

    void SpeakCancel();

    // Speech to text

    // TODO
 }
