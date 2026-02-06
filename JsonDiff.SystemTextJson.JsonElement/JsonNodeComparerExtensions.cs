#if NET8_0_OR_GREATER
namespace NoP77svk.JsonDiff;

using System.Text.Json.Nodes;

public static class JsonNodeComparerExtensions
{
    /// <summary>
    /// Enumerates the differences between two (artbitrary type) JSON documents/nodes.
    /// </summary>
    /// <param name="leftNode">Left JSON document/node.</param>
    /// <param name="rightNode">Right JSON document/node.</param>
    /// <returns>
    ///     <para>An enumerable with differences, one for extra/changed value from the left side, one for extra/changed value from the right side.</para>
    ///     <para>If there's only a "left" side node returned, it means there was no matching "right" side node.</para>
    ///     <para>If there's only a "right" side node returned, it means there was no matching "left" side node.</para>
    /// </returns>
    public static IEnumerable<JsonDifference<JsonNode?>> CompareWith(this JsonNode? leftNode, JsonNode? rightNode)
        => leftNode.CompareWith(rightNode, JsonNodeDiffValuesSelector.DefaultInstance);
}
#endif
