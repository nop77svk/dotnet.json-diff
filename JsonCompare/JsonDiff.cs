namespace NoP77svk.JsonCompare;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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

    public static IEnumerable<Difference<TNode>> CompareWith<TNode>(this TNode? leftDocument, TNode? rightDocument, IJsonDiffNodeValuesSelector<TNode> valueSelector)
        => new JsonDiff<TNode>().EnumerateDifferences(@"$", leftDocument, rightDocument, valueSelector);

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

    public MatchArrayElementsBy ArrayElementMatchingStrategy { get; init; } = MatchArrayElementsBy.Position;
    public MatchObjectPropertiesBy ObjectPropertiesMatchingStrategy { get; init; } = MatchObjectPropertiesBy.Name;

    public JsonDiff()
    {
    }

    public IEnumerable<JsonDiff.Difference<TNode>> EnumerateDifferences(string jsonPath, TNode? leftNode, TNode? rightNode, IJsonDiffNodeValuesSelector<TNode> valueSelector)
    {
        JsonValueKind leftValueKind = valueSelector.GetValueKind(leftNode);
        JsonValueKind rightValueKind = valueSelector.GetValueKind(rightNode);

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
                JsonValueKind.String => EnumerateStringValueDifferences(jsonPath, leftNode, rightNode, valueSelector),
                JsonValueKind.Number => EnumerateNumberValueDifferences(jsonPath, leftNode, rightNode, valueSelector),
                JsonValueKind.Array => EnumerateArrayElementsDifferences(jsonPath, valueSelector.GetArrayValues(leftNode), valueSelector.GetArrayValues(rightNode), valueSelector),
                JsonValueKind.Object => EnumerateObjectPropertiesDifferences(jsonPath, valueSelector.GetObjectProperties(leftNode), valueSelector.GetObjectProperties(rightNode), valueSelector),
                JsonValueKind.Null => Enumerable.Empty<JsonDiff.Difference<TNode>>(),
                _ => throw new NotImplementedException($"Comparison for JSON value kind '{leftValueKind}' is not (yet) implemented."),
            };
        }

        return result;
    }

    private static IEnumerable<JsonDiff.Difference<TNode>> EnumerateValueKindDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
        yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
    }

    private static IEnumerable<JsonDiff.Difference<TNode>> EnumerateNumberValueDifferences(string jsonPath, TNode? leftNode, TNode? rightNode, IJsonDiffNodeValuesSelector<TNode> valueSelector)
    {
        decimal valueLeft = valueSelector.GetNumberValue(leftNode);
        decimal valueRight = valueSelector.GetNumberValue(rightNode);

        if (valueLeft != valueRight)
        {
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
        }
    }

    private static IEnumerable<JsonDiff.Difference<TNode>> EnumerateStringValueDifferences(string jsonPath, TNode? leftNode, TNode? rightNode, IJsonDiffNodeValuesSelector<TNode> valueSelector)
    {
        if (!valueSelector.GetStringValue(leftNode).Equals(valueSelector.GetStringValue(rightNode)))
        {
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Left, leftNode);
            yield return new JsonDiff.Difference<TNode>(jsonPath, JsonDiff.DifferenceSide.Right, rightNode);
        }
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

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateArrayElementsDifferences(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftArray, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightArray, IJsonDiffNodeValuesSelector<TNode> valueSelector)
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
                IEnumerable<JsonDiff.Difference<TNode>> differencesOnThisArrayItem = EnumerateDifferences(arrayItemJsonPath, leftArrayEnumerator.Current.ArrayElement, rightArrayEnumerator.Current.ArrayElement, valueSelector);
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

    private IEnumerable<JsonDiff.Difference<TNode>> EnumerateObjectPropertiesDifferences(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftObjectEnumerable, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightObjectEnumerable, IJsonDiffNodeValuesSelector<TNode> valueSelector)
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
            string propertyJsonPath = JsonPathCombine(jsonPath, property.Key);
            yield return new JsonDiff.Difference<TNode>(propertyJsonPath, JsonDiff.DifferenceSide.Left, property.ArrayElement);
        }

        var rightObjectExtraProperties = rightObjectEnumerable
            .Where(property => !leftObjectGroupedByProperty.Contains(property.Key));

        foreach (var property in rightObjectExtraProperties)
        {
            string propertyJsonPath = JsonPathCombine(jsonPath, property.Key);
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
            string propertyJsonPath = JsonPathCombine(jsonPath, joined.Item1);

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
                differences = EnumerateObjectPropertiesDifferences(propertyJsonPath, leftPropertyValues, rightPropertyValues, valueSelector);
            }
            else
            {
                differences = EnumerateDifferences(propertyJsonPath, leftPropertyValues[0].ArrayElement, rightPropertyValues[0].ArrayElement, valueSelector);
            }

            foreach (var difference in differences)
            {
                yield return difference;
            }
        }
    }

    private static readonly Regex _rxPropertyNameIsClean = new Regex(@"^[a-z_][a-z0-9_]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static string JsonPathCombine(string jsonPath, string propertyName)
    {
        string sanitizedPropertyName = _rxPropertyNameIsClean.IsMatch(propertyName)
            ? propertyName
            : $"\"{propertyName}\"";

        return $"{jsonPath}.{sanitizedPropertyName}";
    }
}
