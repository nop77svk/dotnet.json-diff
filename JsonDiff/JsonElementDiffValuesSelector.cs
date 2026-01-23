namespace NoP77svk.JsonDiff;

using System.Text.Json;

public sealed class JsonElementDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonElement>
{
    /// <summary>
    /// Gets/inits (optional) customizable delegate to calculate the key of the array element at the given index from the given node.
    /// </summary>
    public Func<int, JsonElement, string>? ArrayElementDescriptorSelector { get; init; } = null;

    public JsonElementDiffValuesSelector()
    {
    }

    /// <summary>
    /// Gets a singleton instance of <see cref="JsonElementDiffValuesSelector"/> with no customizations.
    /// </summary>
    public static JsonElementDiffValuesSelector DefaultInstance { get; } = new JsonElementDiffValuesSelector();

    public JsonValueKind GetValueKind(JsonElement node) => node.ValueKind;

    public string GetStringValue(JsonElement node) => node.GetString() ?? string.Empty;

    public decimal GetNumberValue(JsonElement node) => node.GetDecimal();

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonElement>> GetArrayValues(JsonElement node)
        => node.EnumerateArray()
        .Select((element, index) => new JsonDiffArrayElementDescriptor<JsonElement>(index, GetArrayElementKey(index, element), element));

    /// <summary>
    /// Calculate the key of the array element at the given index from the given node via the <see cref="ArrayElementDescriptorSelector"/> delegate.
    /// If the delegate is not set, a default key in the format "element #&lt;index&gt;" is returned.
    /// </summary>
    /// <param name="index">The JSON node's array index</param>
    /// <param name="node">JSON node</param>
    /// <returns>Calculated string key of the array element.</returns>
    public string GetArrayElementKey(int index, JsonElement node)
        => ArrayElementDescriptorSelector?.Invoke(index, node)
        ?? $"element #{index}";

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonElement>> GetObjectProperties(JsonElement node)
        => node.EnumerateObject()
        .Select((property, index) => new JsonDiffArrayElementDescriptor<JsonElement>(index, property.Name, property.Value));
}
