namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class JsonDiffExtensions
{
    public static IEnumerable<JsonDifference<TNode>> CompareWith<TNode>(this TNode? leftDocument, TNode? rightDocument, IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
        => new JsonDiff<TNode>(nodeValuesSelector).EnumerateDifferences(@"$", leftDocument, rightDocument);

    public static IEnumerable<JsonDifference<JsonElement>> CompareWith(this JsonDocument leftDocument, JsonDocument rightDocument)
        => leftDocument.RootElement.CompareWith(rightDocument.RootElement);

    public static IEnumerable<JsonDifference<JsonElement>> CompareWith(this JsonElement leftElement, JsonElement rightElement)
        => leftElement.CompareWith(rightElement, JsonElementDiffValuesSelector.Instance);

    public static IEnumerable<JsonDifference<JsonNode?>> CompareWith(this JsonNode? leftNode, JsonNode? rightNode)
        => leftNode.CompareWith(rightNode, JsonNodeDiffValuesSelector.Instance);
}
