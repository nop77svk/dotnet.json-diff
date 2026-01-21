namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
