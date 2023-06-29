namespace MauiComponents;

public interface ISerializer
{
    ValueTask SerializeAsync(Stream stream, object obj, CancellationToken cancel = default);

    string Serialize(object obj);

    ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancel = default);

    T? Deserialize<T>(string json);
}
