namespace MauiComponents;

using System.Text.Json;

public sealed class JsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions options;

    public JsonSerializer()
        : this(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })
    {
    }

    public JsonSerializer(JsonSerializerOptions options)
    {
        this.options = options;
    }

    public async ValueTask SerializeAsync(Stream stream, object obj, CancellationToken cancel = default) =>
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), options, cancel).ConfigureAwait(false);

    public string Serialize(object obj) =>
        System.Text.Json.JsonSerializer.Serialize(obj, obj.GetType(), options);

    public async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancel = default) =>
        await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, options, cancel).ConfigureAwait(false);

    public T? Deserialize<T>(string json) =>
        System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
}
