namespace NoP77svk.JsonDiff;

using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class JsonNodeDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonNode?>
{
    public Func<int, JsonNode?, string>? ArrayElementDescriptorSelector { get; init; } = null;

    public JsonNodeDiffValuesSelector()
    {
    }

    public static JsonNodeDiffValuesSelector DefaultInstance { get; } = new JsonNodeDiffValuesSelector();

    public JsonValueKind GetValueKind(JsonNode? node) => node?.GetValueKind() ?? JsonValueKind.Null;

    public string GetStringValue(JsonNode? node) => node?.GetValue<string>() ?? string.Empty;

    public decimal GetNumberValue(JsonNode? node) => node?.GetValue<decimal>() ?? 0m;

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonNode?>> GetArrayValues(JsonNode? node)
        => node?.AsArray()
        ?.Select((element, index) => new JsonDiffArrayElementDescriptor<JsonNode?>(index, GetArrayElementKey(index, element), element))
        ?? Enumerable.Empty<JsonDiffArrayElementDescriptor<JsonNode?>>();

    public string GetArrayElementKey(int index, JsonNode? node)
        => ArrayElementDescriptorSelector?.Invoke(index, node)
        ?? $"element #{index}";

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonNode?>> GetObjectProperties(JsonNode? node)
        => node?.AsObject()
        .Select((property, index) => new JsonDiffArrayElementDescriptor<JsonNode?>(index, property.Key, property.Value))
        ?? Enumerable.Empty<JsonDiffArrayElementDescriptor<JsonNode?>>();
}
