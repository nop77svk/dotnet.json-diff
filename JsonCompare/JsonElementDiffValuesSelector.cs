namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

internal class JsonElementDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonElement>
{
    private JsonElementDiffValuesSelector()
    {
    }

    public static JsonElementDiffValuesSelector Instance { get; } = new JsonElementDiffValuesSelector();

    public JsonValueKind GetValueKind(JsonElement node) => node.ValueKind;

    public string GetStringValue(JsonElement node) => node.GetString() ?? string.Empty;

    public decimal GetNumberValue(JsonElement node) => node.GetDecimal();

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonElement>> GetArrayValues(JsonElement node)
        => node.EnumerateArray()
        .Select((element, index) => new JsonDiffArrayElementDescriptor<JsonElement>(index, GetArrayElementDescriptor(index, node), element));

    public string GetArrayElementDescriptor(int index, JsonElement node) => index.ToString();

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonElement>> GetObjectProperties(JsonElement node)
        => node.EnumerateObject()
        .Select((property, index) => new JsonDiffArrayElementDescriptor<JsonElement>(index, property.Name, property.Value));

    IEnumerable<JsonDiffArrayElementDescriptor<JsonElement>> IJsonDiffNodeValuesSelector<JsonElement>.GetArrayValues(JsonElement node)
    {
        throw new NotImplementedException();
    }
}
