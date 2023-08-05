namespace MauiComponents;

using System.Text.Json;

public sealed class DefaultSerializerConfig
{
    public JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

public sealed class DefaultSerializer : ISerializer
{
    private readonly JsonSerializerOptions options;

    public DefaultSerializer()
        : this(new DefaultSerializerConfig())
    {
    }

    public DefaultSerializer(DefaultSerializerConfig config)
    {
        options = config.Options;
    }

    public async ValueTask SerializeAsync(Stream stream, object obj, CancellationToken cancel = default) =>
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), options, cancel).ConfigureAwait(true);

    public string Serialize(object obj) =>
        System.Text.Json.JsonSerializer.Serialize(obj, obj.GetType(), options);

    public async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancel = default) =>
        await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, options, cancel).ConfigureAwait(true);

    public T? Deserialize<T>(string json) =>
        System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
}
