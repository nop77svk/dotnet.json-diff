namespace NoP77svk.JsonDiff;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

public class JsonComparer<TNode>
{
    private readonly IJsonDiffNodeValuesSelector<TNode> _nodeValuesSelector;

    public MatchJsonArrayElementsBy ArrayElementMatchingStrategy { get; init; } = MatchJsonArrayElementsBy.Position;

    public MatchJsonObjectPropertiesBy ObjectPropertiesMatchingStrategy { get; init; } = MatchJsonObjectPropertiesBy.Name;

    public JsonComparer(IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
    {
        _nodeValuesSelector = nodeValuesSelector;
    }

    public IEnumerable<JsonDifference<TNode>> EnumerateDifferences(TNode? leftNode, TNode? rightNode)
        => EnumerateDifferences(jsonPath: "$", leftNode: leftNode, rightNode: rightNode);

    private static IEnumerable<JsonDifference<TNode>> EnumerateBooleanNodeDifferences(string jsonPath, TNode? leftNode, TNode? rightNode, JsonValueKind leftValueKind, JsonValueKind rightValueKind)
    {
        bool valueLeft = leftValueKind is JsonValueKind.True;
        bool valueRight = rightValueKind is JsonValueKind.True;

        if (valueLeft != valueRight)
        {
            yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Left, leftNode);
            yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Right, rightNode);
        }
    }

    private static IEnumerable<JsonDifference<TNode>> EnumerateValueKindDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Left, leftNode);
        yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Right, rightNode);
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        JsonValueKind leftValueKind = _nodeValuesSelector.GetValueKind(leftNode);
        JsonValueKind rightValueKind = _nodeValuesSelector.GetValueKind(rightNode);

        IEnumerable<JsonDifference<TNode>> result;

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
                JsonValueKind.Array => EnumerateArrayElementsDifferences(jsonPath, _nodeValuesSelector.GetArrayValues(leftNode), _nodeValuesSelector.GetArrayValues(rightNode)),
                JsonValueKind.Object => EnumerateObjectPropertiesDifferences(jsonPath, _nodeValuesSelector.GetObjectProperties(leftNode), _nodeValuesSelector.GetObjectProperties(rightNode)),
                JsonValueKind.Null => Enumerable.Empty<JsonDifference<TNode>>(),
                _ => throw new NotImplementedException($"Comparison for JSON value kind '{leftValueKind}' is not (yet) implemented."),
            };
        }

        return result;
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateNumberValueDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        decimal valueLeft = _nodeValuesSelector.GetNumberValue(leftNode);
        decimal valueRight = _nodeValuesSelector.GetNumberValue(rightNode);

        if (valueLeft != valueRight)
        {
            yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Left, leftNode);
            yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Right, rightNode);
        }
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateStringValueDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        if (!_nodeValuesSelector.GetStringValue(leftNode).Equals(_nodeValuesSelector.GetStringValue(rightNode)))
        {
            yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Left, leftNode);
            yield return new JsonDifference<TNode>(jsonPath, JsonDifferenceSide.Right, rightNode);
        }
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateArrayElementsDifferences(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftArray, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightArray)
        => ArrayElementMatchingStrategy switch
        {
            MatchJsonArrayElementsBy.Position => EnumerateArrayElementsDifferencesByPosition(jsonPath, leftArray, rightArray),
            MatchJsonArrayElementsBy.Key => EnumerateArrayElementsDifferencesByKey(jsonPath, leftArray, rightArray),
            _ => throw new NotImplementedException($"Matching array elements by '{ArrayElementMatchingStrategy}' is not (yet) implemented."),
        };

    private IEnumerable<JsonDifference<TNode>> EnumerateArrayElementsDifferencesByPosition(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftArray,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightArray)
    {
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> leftArrayEnumerator = leftArray.GetEnumerator();
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> rightArrayEnumerator = rightArray.GetEnumerator();

        while (true)
        {
            bool leftArrayHasItems = leftArrayEnumerator.MoveNext();
            bool rightArrayHasItems = rightArrayEnumerator.MoveNext();

            if (leftArrayHasItems && rightArrayHasItems)
            {
                string arrayItemJsonPath = $"{jsonPath}[{leftArrayEnumerator.Current.Index}]";
                IEnumerable<JsonDifference<TNode>> differencesOnThisArrayItem = EnumerateDifferences(arrayItemJsonPath, leftArrayEnumerator.Current.ArrayElement, rightArrayEnumerator.Current.ArrayElement);
                foreach (var difference in differencesOnThisArrayItem)
                {
                    yield return difference;
                }
            }
            else if (leftArrayHasItems)
            {
                string arrayItemJsonPath = $"{jsonPath}[{leftArrayEnumerator.Current.Index}]";
                yield return new JsonDifference<TNode>(arrayItemJsonPath, JsonDifferenceSide.Left, leftArrayEnumerator.Current.ArrayElement);
            }
            else if (rightArrayHasItems)
            {
                string arrayItemJsonPath = $"{jsonPath}[{rightArrayEnumerator.Current.Index}]";
                yield return new JsonDifference<TNode>(arrayItemJsonPath, JsonDifferenceSide.Right, rightArrayEnumerator.Current.ArrayElement);
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateArrayElementsDifferencesByKey(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftElements,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightElements)
    {
        var leftElementsGroupedByKey = leftElements
            .ToLookup(
                keySelector: property => property.Key,
                elementSelector: property => property.ArrayElement
            );

        var rightElementsGroupedByKey = rightElements
            .ToLookup(
                keySelector: property => property.Key,
                elementSelector: property => property.ArrayElement
            );

        var leftObjectExtraElements = leftElements
            .Where(element => !rightElementsGroupedByKey.Contains(element.Key));

        foreach (var element in leftObjectExtraElements)
        {
            string elementJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, element.Key);
            yield return new JsonDifference<TNode>(elementJsonPath, JsonDifferenceSide.Left, element.ArrayElement);
        }

        var rightObjectExtraElements = rightElements
            .Where(element => !leftElementsGroupedByKey.Contains(element.Key));

        foreach (var element in rightObjectExtraElements)
        {
            string elementJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, element.Key);
            yield return new JsonDifference<TNode>(elementJsonPath, JsonDifferenceSide.Right, element.ArrayElement);
        }

        var leftAndRightJoinedByKey = leftElementsGroupedByKey
            .Join(
                inner: rightElementsGroupedByKey,
                outerKeySelector: left => left.Key,
                innerKeySelector: right => right.Key,
                resultSelector: (left, right) => new ValueTuple<string, IEnumerable<TNode?>, IEnumerable<TNode?>>(
                    left.Key,
                    left.ToList(),
                    right.ToList()
                )
            );

        foreach (var joined in leftAndRightJoinedByKey)
        {
            string elementJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, joined.Item1);

            var leftElementValues = joined.Item2
                .Select((value, index) => new JsonDiffArrayElementDescriptor<TNode>(index, joined.Item1, value))
                .OrderBy(kvp => kvp.ArrayElement?.GetHashCode() ?? 0)
                .ToList();

            var rightElementValues = joined.Item3
                .Select((value, index) => new JsonDiffArrayElementDescriptor<TNode>(index, joined.Item1, value))
                .OrderBy(kvp => kvp.ArrayElement?.GetHashCode() ?? 0)
                .ToList();

            IEnumerable<JsonDifference<TNode>> differences;
            if (leftElementValues.Count > 1 || rightElementValues.Count > 1)
            {
                differences = EnumerateObjectPropertiesDifferences(elementJsonPath, leftElementValues, rightElementValues);
            }
            else
            {
                differences = EnumerateDifferences(elementJsonPath, leftElementValues[0].ArrayElement, rightElementValues[0].ArrayElement);
            }

            foreach (var difference in differences)
            {
                yield return difference;
            }
        }
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateObjectPropertiesDifferences(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftObjectEnumerable,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightObjectEnumerable)
        => ObjectPropertiesMatchingStrategy switch
        {
            MatchJsonObjectPropertiesBy.Name => EnumerateObjectPropertiesDifferencesByName(jsonPath, leftObjectEnumerable, rightObjectEnumerable),
            MatchJsonObjectPropertiesBy.Position => EnumerateObjectPropertiesDifferencesByPosition(jsonPath, leftObjectEnumerable, rightObjectEnumerable),
            _ => throw new NotImplementedException($"Matching object properties by '{ObjectPropertiesMatchingStrategy}' is not (yet) implemented."),
        };

    private IEnumerable<JsonDifference<TNode>> EnumerateObjectPropertiesDifferencesByPosition(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftElements,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightElements)
    {
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> leftElementsEnumerator = leftElements.GetEnumerator();
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> rightElementsEnumerator = rightElements.GetEnumerator();

        while (true)
        {
            bool leftElementsHasItems = leftElementsEnumerator.MoveNext();
            bool rightElementsHasItems = rightElementsEnumerator.MoveNext();

            if (leftElementsHasItems && rightElementsHasItems)
            {
                string elementJsonPath = $"{jsonPath}[{leftElementsEnumerator.Current.Index}]";
                IEnumerable<JsonDifference<TNode>> differencesOnThisElement = EnumerateDifferences(elementJsonPath, leftElementsEnumerator.Current.ArrayElement, rightElementsEnumerator.Current.ArrayElement);
                foreach (var difference in differencesOnThisElement)
                {
                    yield return difference;
                }
            }
            else if (leftElementsHasItems)
            {
                string elementJsonPath = $"{jsonPath}[{leftElementsEnumerator.Current.Index}]";
                yield return new JsonDifference<TNode>(elementJsonPath, JsonDifferenceSide.Left, leftElementsEnumerator.Current.ArrayElement);
            }
            else if (rightElementsHasItems)
            {
                string elementJsonPath = $"{jsonPath}[{rightElementsEnumerator.Current.Index}]";
                yield return new JsonDifference<TNode>(elementJsonPath, JsonDifferenceSide.Right, rightElementsEnumerator.Current.ArrayElement);
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateObjectPropertiesDifferencesByName(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftObjectEnumerable,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightObjectEnumerable)
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
            yield return new JsonDifference<TNode>(propertyJsonPath, JsonDifferenceSide.Left, property.ArrayElement);
        }

        var rightObjectExtraProperties = rightObjectEnumerable
            .Where(property => !leftObjectGroupedByProperty.Contains(property.Key));

        foreach (var property in rightObjectExtraProperties)
        {
            string propertyJsonPath = JsonDiffHelpers.JsonPathCombine(jsonPath, property.Key);
            yield return new JsonDifference<TNode>(propertyJsonPath, JsonDifferenceSide.Right, property.ArrayElement);
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

            IEnumerable<JsonDifference<TNode>> differences;
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

public class JsonElementComparer : JsonComparer<JsonElement>
{
    public JsonElementComparer()
        : base(JsonElementDiffValuesSelector.DefaultInstance)
    {
    }
}

public class JsonNodeComparer : JsonComparer<JsonNode?>
{
    public JsonNodeComparer()
        : base(JsonNodeDiffValuesSelector.DefaultInstance)
    {
    }
}
