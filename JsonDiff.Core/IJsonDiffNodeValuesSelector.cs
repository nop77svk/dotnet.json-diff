namespace NoP77svk.JsonDiff;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record JsonDiffArrayElementDescriptor<TNode>(int Index, string Key, TNode? ArrayElement);
#pragma warning restore SA1313

/// <summary>
/// The interface to select values from JSON nodes of type TNode.
/// </summary>
/// <typeparam name="TNode">Specific JSON node type to be used.</typeparam>
public interface IJsonDiffNodeValuesSelector<TNode>
{
    /// <summary>
    /// Get the kind of JSON value represented by the given node.
    /// </summary>
    /// <param name="node">JSON node</param>
    /// <returns>JSON node value kind</returns>
    JsonDiffValueKind GetValueKind(TNode? node);

    /// <summary>
    /// Get the string value represented by the given node (or null if the node is null).
    /// </summary>
    /// <param name="node">JSON node</param>
    /// <returns>String value of the JSON node (i.e., a property or an array element)</returns>
    string GetStringValue(TNode? node);

    /// <summary>
    /// Get the number value represented by the given node as decimal (or null if the node is null).
    /// </summary>
    /// <param name="node">JSON node</param>
    /// <returns>Decimal number value of the JSON node (i.e., a property or an array element)</returns>
    decimal GetNumberValue(TNode? node);

    bool GetBooleanValue(TNode? node);

    /// <summary>
    /// Enumerate array values from the given node. Each value is represented by its index, (calculated by <see cref="GetArrayElementKey(int, TNode?)"/>) key, and the actual JSON node.
    /// </summary>
    /// <param name="node">JSON node</param>
    /// <returns>Enumeration of the array values of the JSON node.</returns>
    IEnumerable<JsonDiffArrayElementDescriptor<TNode>> GetArrayValues(TNode? node);

    /// <summary>
    /// Calculate the key of the array element at the given index from the given node. This is subject to custom implementation (e.g., based on some property of the array element).
    /// </summary>
    /// <param name="index">The JSON node's array index</param>
    /// <param name="node">JSON node</param>
    /// <returns>Calculated string key of the array element.</returns>
    string GetArrayElementKey(int index, TNode? node);

    /// <summary>
    /// Enumerate properties from the given node. Each property is represented by its index, name (as "Key"), and the actual JSON node.
    /// </summary>
    /// <param name="node">JSON node</param>
    /// <returns>Enumeration of the array values of the JSON node.</returns>
    IEnumerable<JsonDiffArrayElementDescriptor<TNode>> GetObjectProperties(TNode? node);
}
