
using System.Text.Json;

public sealed class JsonDataLoader
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<T> Load<T>(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? [];
    }
}
