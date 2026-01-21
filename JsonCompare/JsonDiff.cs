namespace NoP77svk.JsonCompare;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class JsonDiff
{
    public enum DifferenceSide
    {
        Left,
        Right
    }

#pragma warning disable SA1313
    public record Difference<TNode>(string NodePath, DifferenceSide Side, TNode? NodeValue);
#pragma warning restore SA1313

    public static IEnumerable<Difference<TNode>> CompareWith<TNode>(this TNode? leftDocument, TNode? rightDocument, IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
        => new JsonDiff<TNode>(nodeValuesSelector).EnumerateDifferences(@"$", leftDocument, rightDocument);

    public static IEnumerable<Difference<JsonElement>> CompareWith(this JsonDocument leftDocument, JsonDocument rightDocument)
        => leftDocument.RootElement.CompareWith(rightDocument.RootElement);

    public static IEnumerable<Difference<JsonElement>> CompareWith(this JsonElement leftElement, JsonElement rightElement)
        => leftElement.CompareWith(rightElement, JsonElementDiffValuesSelector.Instance);

    public static IEnumerable<Difference<JsonNode?>> CompareWith(this JsonNode? leftNode, JsonNode? rightNode)
        => leftNode.CompareWith(rightNode, JsonNodeDiffValuesSelector.Instance);
}

public class JsonDiff<TNode>
{
    public enum MatchArrayElementsBy
    {
        Position,
        Key
    }

    public enum MatchObjectPropertiesBy
    {
        Name,
        Position
    }

    public IJsonDiffNodeValuesSelector<TNode> NodeValuesSelector { get; }
    public MatchArrayElementsBy ArrayElementMatchingStrategy { get; init; } = MatchArrayElementsBy.Position;
    public MatchObjectPropertiesBy ObjectPropertiesMatchingStrategy { get; init; } = MatchObjectPropertiesBy.Name;

    public JsonDiff(IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
    {
        NodeValuesSelector = nodeValuesSelector;
    }

    public IEnumerable<JsonDiff.Difference<TNode>> EnumerateDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        JsonValueKind leftValueKind = NodeValuesSelector.GetValueKind(leftNode);
        JsonValueKind rightValueKind = NodeValuesSelector.GetValueKind(rightNode);

        IEnumerable<JsonDiff.Difference<TNode>> result;

        if (leftValueKind is JsonValueKind.True or JsonValueKind.False
            && rightValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            result = EnumerateBooleanNodeDifferences(jsonPath, leftNode, rightNode, leftValueKind, rightValueKind);
        }
        else if (leftValueKind != rightValueKind)
        {
            result = EnumerateValueKindDifferences(jsonPath, leftNode, rightNode);
        }
        else
        {
            result = leftValueKind switch
            {
                JsonValueKind.String => EnumerateStringValueDifferences(jsonPath, leftNode, rightNode),
                JsonValueKind.Number => EnumerateNumberValueDifferences(jsonPath, leftNode, rightNode),
                JsonValueKind.Array => EnumerateArrayElementsDifferences(jsonPath, NodeValuesSelector.GetArrayValues(leftNode), NodeValuesSelector.GetArrayValues(rightNode)),
                JsonValueKind.Object => EnumerateObjectPropertiesDifferences(jsonPath, NodeValuesSelector.GetObjectProperties(leftNode), NodeValuesSelector.GetObjectProperties(rightNode)),
                JsonValueKind.Null => Enumerable.Empty<JsonDiff.Difference<TNode>>(),
                _ => throw new NotImplementedException($"Comparison for JSON value kind '{leftValueKind}' is not (yet) implemented."),
            };
        }

        return result;
    }

    private static IEnumerable<JsonDiff.Difference<TNode>> EnumerateBooleanNodeDifferences(string jsonPath, TNode? leftNode, TNode? rightNode, JsonValueKind leftValueKind, JsonValueKind rightValueKind)
    {
        bool valueLeft = leftValueKind is JsonValueKind.True;
        bool valueRight = rightValueKind is JsonValueKind.True;

        if (valueLeft != valueRight)
        {
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
        }
    }

    private static IEnumerable<JsonDiff.Difference<TNode>> EnumerateValueKindDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
        yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
    }

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateNumberValueDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        decimal valueLeft = NodeValuesSelector.GetNumberValue(leftNode);
        decimal valueRight = NodeValuesSelector.GetNumberValue(rightNode);

        if (valueLeft != valueRight)
        {
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
        }
    }

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateStringValueDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        if (!NodeValuesSelector.GetStringValue(leftNode).Equals(NodeValuesSelector.GetStringValue(rightNode)))
        {
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
        }
    }

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateArrayElementsDifferences(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftArray, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightArray)
        => ArrayElementMatchingStrategy switch
        {
            MatchArrayElementsBy.Position => EnumerateArrayElementsDifferencesByPosition(jsonPath, leftArray, rightArray),
            _ => throw new NotImplementedException($"Matching array elements by '{ArrayElementMatchingStrategy}' is not (yet) implemented."),
        };

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateArrayElementsDifferencesByPosition(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftArray, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightArray)
    {
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> leftArrayEnumerator = leftArray.GetEnumerator();
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> rightArrayEnumerator = rightArray.GetEnumerator();

        int arrayItemIndex = 0;

        while (true)
        {
            bool leftArrayHasItems = leftArrayEnumerator.MoveNext();
            bool rightArrayHasItems = rightArrayEnumerator.MoveNext();

            string arrayItemJsonPath = $"{jsonPath}[{arrayItemIndex}]";

            if (leftArrayHasItems && rightArrayHasItems)
            {
                IEnumerable<JsonDiff.Difference<TNode>> differencesOnThisArrayItem = EnumerateDifferences(arrayItemJsonPath, leftArrayEnumerator.Current.ArrayElement, rightArrayEnumerator.Current.ArrayElement);
                foreach (var difference in differencesOnThisArrayItem)
                {
                    yield return difference;
                }
            }
            else if (leftArrayHasItems)
            {
                yield return new JsonDiff.Difference<TNode>(arrayItemJsonPath, JsonDiff.DifferenceSide.Left, leftArrayEnumerator.Current.ArrayElement);
            }
            else if (rightArrayHasItems)
            {
                yield return new JsonDiff.Difference<TNode>(arrayItemJsonPath, JsonDiff.DifferenceSide.Right, rightArrayEnumerator.Current.ArrayElement);
            }
            else
            {
                break;
            }

            arrayItemIndex++;
        }
    }

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateObjectPropertiesDifferences(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftObjectEnumerable, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightObjectEnumerable)
        => ObjectPropertiesMatchingStrategy switch
        {
            MatchObjectPropertiesBy.Name => EnumerateObjectPropertiesDifferencesByName(jsonPath, leftObjectEnumerable, rightObjectEnumerable),
            _ => throw new NotImplementedException($"Matching object properties by '{ObjectPropertiesMatchingStrategy}' is not (yet) implemented."),
        };

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateObjectPropertiesDifferencesByName(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftObjectEnumerable, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightObjectEnumerable)
    {
        var leftObjectGroupedByProperty = leftObjectEnumerable
            .ToLookup(
                keySelector: property => property.Key,
                elementSelector: property => property.ArrayElement
            );

        var rightObjectGroupedByProperty = rightObjectEnumerable
            .ToLookup(
                keySelector: property => property.Key,
                elementSelector: property => property.ArrayElement
            );

        var leftObjectExtraProperties = leftObjectEnumerable
            .Where(property => !rightObjectGroupedByProperty.Contains(property.Key));

        foreach (var property in leftObjectExtraProperties)
        {
            string propertyJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, property.Key);
            yield return new JsonDiff.Difference<TNode>(propertyJsonPath, JsonDiff.DifferenceSide.Left, property.ArrayElement);
        }

        var rightObjectExtraProperties = rightObjectEnumerable
            .Where(property => !leftObjectGroupedByProperty.Contains(property.Key));

        foreach (var property in rightObjectExtraProperties)
        {
            string propertyJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, property.Key);
            yield return new JsonDiff.Difference<TNode>(propertyJsonPath, JsonDiff.DifferenceSide.Right, property.ArrayElement);
        }

        var leftAndRightJoinedByPropertyName = leftObjectGroupedByProperty
            .Join(
                inner: rightObjectGroupedByProperty,
                outerKeySelector: left => left.Key,
                innerKeySelector: right => right.Key,
                resultSelector: (left, right) => new ValueTuple<string, IEnumerable<TNode?>, IEnumerable<TNode?>>(
                    left.Key,
                    left.ToList(),
                    right.ToList()
                )
            );

        foreach (var joined in leftAndRightJoinedByPropertyName)
        {
            string propertyJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, joined.Item1);

            var leftPropertyValues = joined.Item2
                .Select((value, index) => new JsonDiffArrayElementDescriptor<TNode>(index, joined.Item1, value))
                .OrderBy(kvp => kvp.ArrayElement?.GetHashCode() ?? 0)
                .ToList();

            var rightPropertyValues = joined.Item3
                .Select((value, index) => new JsonDiffArrayElementDescriptor<TNode>(index, joined.Item1, value))
                .OrderBy(kvp => kvp.ArrayElement?.GetHashCode() ?? 0)
                .ToList();

            IEnumerable<JsonDiff.Difference<TNode>> differences;
            if (leftPropertyValues.Count > 1 || rightPropertyValues.Count > 1)
            {
                differences = EnumerateObjectPropertiesDifferences(propertyJsonPath, leftPropertyValues, rightPropertyValues);
            }
            else
            {
                differences = EnumerateDifferences(propertyJsonPath, leftPropertyValues[0].ArrayElement, rightPropertyValues[0].ArrayElement);
            }

            foreach (var difference in differences)
            {
                yield return difference;
            }
        }
    }
}
