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
    public record Difference<TNode>(string NodePath, DifferenceSide Side, TNode NodeValue);
#pragma warning restore SA1313

    public static IEnumerable<Difference<JsonElement>> CompareWith(this JsonDocument leftDocument, JsonDocument rightDocument)
        => leftDocument.RootElement.CompareWith(rightDocument.RootElement);

    public static IEnumerable<Difference<JsonElement>> CompareWith(this JsonElement leftElement, JsonElement rightElement)
        => EnumerateDifferences(@"$", leftElement, rightElement, JsonElementDiffValuesSelector.Instance);

    public static IEnumerable<Difference<JsonNode>> CompareWith(this JsonNode leftNode, JsonNode rightNode)
        => EnumerateDifferences(@"$", leftNode, rightNode, JsonNodeDiffValuesSelector.Instance);

    private static IEnumerable<Difference<TNode>> EnumerateDifferences<TNode>(string jsonPath, TNode leftNode, TNode rightNode, IJsonDiffNodeValuesSelector<TNode> valueSelector)
    {
        JsonValueKind leftValueKind = valueSelector.GetValueKind(leftNode);
        JsonValueKind rightValueKind = valueSelector.GetValueKind(rightNode);

        if (leftValueKind is JsonValueKind.True or JsonValueKind.False
            && rightValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            bool valueLeft = leftValueKind is JsonValueKind.True;
            bool valueRight = rightValueKind is JsonValueKind.True;

            if (valueLeft != valueRight)
            {
                yield return new Difference<TNode>(jsonPath, DifferenceSide.Left, leftNode);
                yield return new Difference<TNode>(jsonPath, DifferenceSide.Right, rightNode);
            }
        }
        else if (leftValueKind != rightValueKind)
        {
            yield return new Difference<TNode>(jsonPath, DifferenceSide.Left, leftNode);
            yield return new Difference<TNode>(jsonPath, DifferenceSide.Right, rightNode);
        }
        else if (leftValueKind is JsonValueKind.String)
        {
            if (!valueSelector.GetStringValue(leftNode).Equals(valueSelector.GetStringValue(rightNode)))
            {
                yield return new Difference<TNode>(jsonPath, DifferenceSide.Left, leftNode);
                yield return new Difference<TNode>(jsonPath, DifferenceSide.Right, rightNode);
            }
        }
        else if (leftValueKind is JsonValueKind.Number)
        {
            decimal valueLeft = valueSelector.GetNumberValue(leftNode);
            decimal valueRight = valueSelector.GetNumberValue(rightNode);

            if (valueLeft != valueRight)
            {
                yield return new Difference<TNode>(jsonPath, DifferenceSide.Left, leftNode);
                yield return new Difference<TNode>(jsonPath, DifferenceSide.Right, rightNode);
            }
        }
        else if (leftValueKind is JsonValueKind.Array)
        {
            foreach (var difference in EnumerateDifferences(jsonPath, valueSelector.GetArrayValues(leftNode), valueSelector.GetArrayValues(rightNode), valueSelector))
            {
                yield return difference;
            }
        }
        else if (leftValueKind is JsonValueKind.Object)
        {
            foreach (var difference in EnumerateDifferences(jsonPath, valueSelector.GetObjectProperties(leftNode), valueSelector.GetObjectProperties(rightNode), valueSelector))
            {
                yield return difference;
            }
        }
        else if (leftValueKind is JsonValueKind.Null)
        {
            // NULLs are equal
        }
        else
        {
            throw new NotImplementedException($"Comparison for JSON value kind '{leftValueKind}' is not (yet) implemented.");
        }
    }

    private static IEnumerable<Difference<TNode>> EnumerateDifferences<TNode>(string jsonPath, IEnumerable<KeyValuePair<int, TNode>> leftArray, IEnumerable<KeyValuePair<int, TNode>> rightArray, IJsonDiffNodeValuesSelector<TNode> valueSelector)
    {
        using IEnumerator<KeyValuePair<int, TNode>> leftArrayEnumerator = leftArray.GetEnumerator();
        using IEnumerator<KeyValuePair<int, TNode>> rightArrayEnumerator = rightArray.GetEnumerator();

        int arrayItemIndex = 0;

        while (true)
        {
            bool leftArrayHasItems = leftArrayEnumerator.MoveNext();
            bool rightArrayHasItems = rightArrayEnumerator.MoveNext();

            string arrayItemJsonPath = $"{jsonPath}[{arrayItemIndex}]";

            if (leftArrayHasItems && rightArrayHasItems)
            {
                IEnumerable<Difference<TNode>> differencesOnThisArrayItem = EnumerateDifferences(arrayItemJsonPath, leftArrayEnumerator.Current.Value, rightArrayEnumerator.Current.Value, valueSelector);
                foreach (var difference in differencesOnThisArrayItem)
                {
                    yield return difference;
                }
            }
            else if (leftArrayHasItems)
            {
                yield return new Difference<TNode>(arrayItemJsonPath, DifferenceSide.Left, leftArrayEnumerator.Current.Value);
            }
            else if (rightArrayHasItems)
            {
                yield return new Difference<TNode>(arrayItemJsonPath, DifferenceSide.Right, rightArrayEnumerator.Current.Value);
            }
            else
            {
                break;
            }

            arrayItemIndex++;
        }
    }

    private static IEnumerable<Difference<TNode>> EnumerateDifferences<TNode>(string jsonPath, IEnumerable<KeyValuePair<string, TNode>> leftObjectEnumerable, IEnumerable<KeyValuePair<string, TNode>> rightObjectEnumerable, IJsonDiffNodeValuesSelector<TNode> valueSelector)
    {
        var leftObjectGroupedByProperty = leftObjectEnumerable
            .ToLookup(
                keySelector: property => property.Key,
                elementSelector: property => property.Value
            );

        var rightObjectGroupedByProperty = rightObjectEnumerable
            .ToLookup(
                keySelector: property => property.Key,
                elementSelector: property => property.Value
            );

        var leftObjectExtraProperties = leftObjectEnumerable
            .Where(property => !rightObjectGroupedByProperty.Contains(property.Key));

        foreach (var property in leftObjectExtraProperties)
        {
            string propertyJsonPath = JsonPathCombine(jsonPath, property.Key);
            yield return new Difference<TNode>(propertyJsonPath, DifferenceSide.Left, property.Value);
        }

        var rightObjectExtraProperties = rightObjectEnumerable
            .Where(property => !leftObjectGroupedByProperty.Contains(property.Key));

        foreach (var property in rightObjectExtraProperties)
        {
            string propertyJsonPath = JsonPathCombine(jsonPath, property.Key);
            yield return new Difference<TNode>(propertyJsonPath, DifferenceSide.Right, property.Value);
        }

        var leftAndRightJoinedByPropertyName = leftObjectGroupedByProperty
            .Join(
                inner: rightObjectGroupedByProperty,
                outerKeySelector: left => left.Key,
                innerKeySelector: right => right.Key,
                resultSelector: (left, right) => new ValueTuple<string, IEnumerable<TNode>, IEnumerable<TNode>>(
                    left.Key,
                    left,
                    right
                )
            );

        foreach (var joined in leftAndRightJoinedByPropertyName)
        {
            string propertyJsonPath = JsonPathCombine(jsonPath, joined.Item1);

            var leftPropertyValues = joined.Item2
                .Select(value => new KeyValuePair<string, TNode>(joined.Item1, value))
                .OrderBy(kvp => kvp.Value?.GetHashCode() ?? 0)
                .ToList();

            var rightPropertyValues = joined.Item3
                .Select(value => new KeyValuePair<string, TNode>(joined.Item1, value))
                .OrderBy(kvp => kvp.Value?.GetHashCode() ?? 0)
                .ToList();

            IEnumerable<Difference<TNode>> differences;
            if (leftPropertyValues.Count > 1 || rightPropertyValues.Count > 1)
            {
                differences = EnumerateDifferences(propertyJsonPath, leftPropertyValues, rightPropertyValues, valueSelector);
            }
            else
            {
                differences = EnumerateDifferences(propertyJsonPath, leftPropertyValues[0].Value, rightPropertyValues[0].Value, valueSelector);
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