namespace NoP77svk.JsonDiff;

using System.Text.Json;
using System.Text.Json.Nodes;

public static class JsonComparerExtensions
{
    public static IEnumerable<JsonDifference<TNode>> CompareWith<TNode>(this TNode? leftDocument, TNode? rightDocument, IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
        => new JsonComparer<TNode>(nodeValuesSelector).EnumerateDifferences(leftDocument, rightDocument);

    public static IEnumerable<JsonDifference<JsonElement>> CompareWith(this JsonDocument leftDocument, JsonDocument rightDocument)
        => leftDocument.RootElement.CompareWith(rightDocument.RootElement);

    public static IEnumerable<JsonDifference<JsonElement>> CompareWith(this JsonElement leftElement, JsonElement rightElement)
        => leftElement.CompareWith(rightElement, JsonElementDiffValuesSelector.DefaultInstance);

#if NET8_0_OR_GREATER
    public static IEnumerable<JsonDifference<JsonNode?>> CompareWith(this JsonNode? leftNode, JsonNode? rightNode)
        => leftNode.CompareWith(rightNode, JsonNodeDiffValuesSelector.DefaultInstance);
#endif
}
