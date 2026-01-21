namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

internal class JsonNodeDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonNode?>
{
    private JsonNodeDiffValuesSelector()
    {
    }

    public static JsonNodeDiffValuesSelector Instance { get; } = new JsonNodeDiffValuesSelector();

    public JsonValueKind GetValueKind(JsonNode? node) => node?.GetValueKind() ?? JsonValueKind.Null;

    public string GetStringValue(JsonNode? node) => node?.GetValue<string>() ?? string.Empty;

    public decimal GetNumberValue(JsonNode? node) => node?.GetValue<decimal>() ?? 0m;

    public IEnumerable<KeyValuePair<int, JsonNode?>> GetArrayValues(JsonNode? node)
        => node?.AsArray()
        .Select((element, index) => new KeyValuePair<int, JsonNode?>(index, element))
        ?? Array.Empty<KeyValuePair<int, JsonNode?>>();

    public IEnumerable<KeyValuePair<string, JsonNode?>> GetObjectProperties(JsonNode? node)
        => node?.AsObject()
        .Select(property => new KeyValuePair<string, JsonNode?>(property.Key, property.Value))
        ?? Array.Empty<KeyValuePair<string, JsonNode?>>();
}
