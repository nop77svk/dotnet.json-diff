#if NET8_0_OR_GREATER
namespace NoP77svk.JsonDiff;

using System.Text.Json.Nodes;

/// <summary>
/// JSON node values selector (<see cref="IJsonDiffFormatter{TNode}"/>) for <see cref="JsonNode"/> type.
/// </summary>
public sealed class JsonNodeDiffValuesSelector : IJsonDiffNodeValuesSelector<JsonNode?>
{
    /// <summary>
    /// Gets/inits (optional) customizable delegate to calculate the key of the array element at the given index from the given node.
    /// </summary>
    public Func<int, JsonNode?, string>? ArrayElementDescriptorSelector { get; init; } = null;

    public JsonNodeDiffValuesSelector()
    {
    }

    /// <summary>
    /// Gets a singleton instance of <see cref="JsonNodeDiffValuesSelector"/> with no customizations.
    /// </summary>
    public static JsonNodeDiffValuesSelector DefaultInstance { get; } = new JsonNodeDiffValuesSelector();

    public JsonDiffValueKind GetValueKind(JsonNode? node) => node?.GetValueKind().ToInternalValueKind() ?? JsonDiffValueKind.Null;

    public bool GetBooleanValue(JsonNode? node) => node?.GetValue<bool>() ?? false;

    public string GetStringValue(JsonNode? node) => node?.GetValue<string>() ?? string.Empty;

    public decimal GetNumberValue(JsonNode? node) => node?.GetValue<decimal>() ?? 0m;

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonNode?>> GetArrayValues(JsonNode? node)
        => node?.AsArray()
        ?.Select((element, index) => new JsonDiffArrayElementDescriptor<JsonNode?>(index, GetArrayElementKey(index, element), element))
        ?? Enumerable.Empty<JsonDiffArrayElementDescriptor<JsonNode?>>();

    /// <summary>
    /// Calculate the key of the array element at the given index from the given node via the <see cref="ArrayElementDescriptorSelector"/> delegate.
    /// If the delegate is not set, a default key in the format "element #&lt;index&gt;" is returned.
    /// </summary>
    /// <param name="index">The JSON node's array index</param>
    /// <param name="node">JSON node</param>
    /// <returns>Calculated string key of the array element.</returns>
    public string GetArrayElementKey(int index, JsonNode? node)
        => ArrayElementDescriptorSelector?.Invoke(index, node)
        ?? $"element #{index}";

    public IEnumerable<JsonDiffArrayElementDescriptor<JsonNode?>> GetObjectProperties(JsonNode? node)
        => node?.AsObject()
        .Select((property, index) => new JsonDiffArrayElementDescriptor<JsonNode?>(index, property.Key, property.Value))
        ?? Enumerable.Empty<JsonDiffArrayElementDescriptor<JsonNode?>>();
}
#endif
