namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

internal interface IJsonDiffNodeValuesSelector<TNode>
{
    JsonValueKind GetValueKind(TNode node);
    string GetStringValue(TNode node);
    decimal GetNumberValue(TNode node);
    IEnumerable<KeyValuePair<int, TNode>> GetArrayValues(TNode node);
    IEnumerable<KeyValuePair<string, TNode>> GetObjectProperties(TNode node);
}

internal class JsonElementDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonElement>
{
    private JsonElementDiffValuesSelector()
    {
    }

    public static JsonElementDiffValuesSelector Instance { get; } = new JsonElementDiffValuesSelector();

    public JsonValueKind GetValueKind(JsonElement node) => node.ValueKind;

    public string GetStringValue(JsonElement node) => node.GetString() ?? string.Empty;

    public decimal GetNumberValue(JsonElement node) => node.GetDecimal();

    public IEnumerable<KeyValuePair<int, JsonElement>> GetArrayValues(JsonElement node)
        => node.EnumerateArray()
        .Select((element, index) => new KeyValuePair<int, JsonElement>(index, element));

    public IEnumerable<KeyValuePair<string, JsonElement>> GetObjectProperties(JsonElement node)
        => node.EnumerateObject()
        .Select(property => new KeyValuePair<string, JsonElement>(property.Name, property.Value));
}

internal class JsonNodeDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonNode>
{
    private JsonNodeDiffValuesSelector()
    {
    }

    public static JsonNodeDiffValuesSelector Instance { get; } = new JsonNodeDiffValuesSelector();

    public JsonValueKind GetValueKind(JsonNode node) => node?.GetValueKind() ?? JsonValueKind.Null;

    public string GetStringValue(JsonNode node) => node?.GetValue<string>() ?? string.Empty;

    public decimal GetNumberValue(JsonNode node) => node?.GetValue<decimal>() ?? 0m;

    public IEnumerable<KeyValuePair<int, JsonNode>> GetArrayValues(JsonNode node)
        => node?.AsArray()
        .Select((element, index) => new KeyValuePair<int, JsonNode>(index, element))
        ?? Array.Empty<KeyValuePair<int, JsonNode>>();

    public IEnumerable<KeyValuePair<string, JsonNode>> GetObjectProperties(JsonNode node)
        => node?.AsObject()
        .Select(property => new KeyValuePair<string, JsonNode>(property.Key, property.Value))
        ?? Array.Empty<KeyValuePair<string, JsonNode>>();
}
