namespace NoP77svk.JsonDiff;

/// <summary>
/// The core JSON comparer implementation.
/// </summary>
/// <typeparam name="TNode">Either JsonNode or JsonElement... or anything else, possibly.</typeparam>
public class JsonComparer<TNode>
{
    private readonly IJsonDiffNodeValuesSelector<TNode> _nodeValuesSelector;

    /// <summary>
    /// Gets/inits the strategy to match JSON array elements<list type="bullet"><item>either by the element position/index (default),</item><item>or by the element key (calculated via supplied node values selector's <c>GetArrayElementKey()</c> callback).</item></list>
    /// </summary>
    public MatchJsonArrayElementsBy ArrayElementMatchingStrategy { get; init; } = MatchJsonArrayElementsBy.Position;

    /// <summary>
    /// Gets/inits the strategy to match JSON object properties<list type="bullet"><item>either by the property name (default),</item><item>or by the property position/index.</item></list>
    /// </summary>
    public MatchJsonObjectPropertiesBy ObjectPropertiesMatchingStrategy { get; init; } = MatchJsonObjectPropertiesBy.Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonComparer{TNode}"/> class.
    /// </summary>
    /// <param name="nodeValuesSelector">The instance of the actual "worked" that reads the properties, arrays, property values from the <typeparamref name="TNode"/> JSON type.</param>
    public JsonComparer(IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
    {
        _nodeValuesSelector = nodeValuesSelector;
    }

    /// <summary>
    /// Enumerates the differences between two JSON documents/nodes.
    /// </summary>
    /// <param name="leftNode">Left JSON document/node.</param>
    /// <param name="rightNode">Right JSON document/node.</param>
    /// <returns>
    ///     <para>An enumerable with differences, one for extra/changed value from the left side, one for extra/changed value from the right side.</para>
    ///     <para>If there's only a "left" side node returned, it means there was no matching "right" side node.</para>
    ///     <para>If there's only a "right" side node returned, it means there was no matching "left" side node.</para>
    /// </returns>
    public IEnumerable<JsonDifference<TNode>> EnumerateDifferences(TNode? leftNode, TNode? rightNode)
        => EnumerateDifferences(jsonPath: "$", leftNode: leftNode, rightNode: rightNode);

    private IEnumerable<JsonDifference<TNode>> EnumerateBooleanNodeDifferences(string jsonPath, TNode? leftNode, TNode? rightNode)
    {
        bool valueLeft = _nodeValuesSelector.GetBooleanValue(leftNode);
        bool valueRight = _nodeValuesSelector.GetBooleanValue(rightNode);

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
        JsonDiffValueKind leftValueKind = _nodeValuesSelector.GetValueKind(leftNode);
        JsonDiffValueKind rightValueKind = _nodeValuesSelector.GetValueKind(rightNode);

        IEnumerable<JsonDifference<TNode>> result;

        if (leftValueKind != rightValueKind)
        {
            result = EnumerateValueKindDifferences(jsonPath, leftNode, rightNode);
        }
        else
        {
            result = leftValueKind switch
            {
                JsonDiffValueKind.Boolean => EnumerateBooleanNodeDifferences(jsonPath, leftNode, rightNode),
                JsonDiffValueKind.String => EnumerateStringValueDifferences(jsonPath, leftNode, rightNode),
                JsonDiffValueKind.Number => EnumerateNumberValueDifferences(jsonPath, leftNode, rightNode),
                JsonDiffValueKind.Array => EnumerateArrayElementsDifferences(jsonPath, _nodeValuesSelector.GetArrayValues(leftNode), _nodeValuesSelector.GetArrayValues(rightNode)),
                JsonDiffValueKind.Object => EnumerateObjectPropertiesDifferences(jsonPath, _nodeValuesSelector.GetObjectProperties(leftNode), _nodeValuesSelector.GetObjectProperties(rightNode)),
                JsonDiffValueKind.Null => Enumerable.Empty<JsonDifference<TNode>>(),
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

    private IEnumerable<JsonDifference<TNode>> EnumerateArrayElementsDifferences(string jsonPath, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftArrayElements, IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightArrayElements)
        => ArrayElementMatchingStrategy switch
        {
            MatchJsonArrayElementsBy.Position => EnumerateElementsDifferencesByPosition(jsonPath, leftArrayElements, rightArrayElements, JsonDiffHelpers.JsonPathCombineWithArrayIndex),
            MatchJsonArrayElementsBy.Key => EnumerateElementsDifferencesByKey(jsonPath, leftArrayElements, rightArrayElements, JsonDiffHelpers.JsonPathCombineWithArrayKey),
            _ => throw new NotImplementedException($"Matching array elements by '{ArrayElementMatchingStrategy}' is not (yet) implemented."),
        };

    private IEnumerable<JsonDifference<TNode>> EnumerateObjectPropertiesDifferences(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftObjectProperties,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightObjectProperties)
        => ObjectPropertiesMatchingStrategy switch
        {
            MatchJsonObjectPropertiesBy.Position => EnumerateElementsDifferencesByPosition(jsonPath, leftObjectProperties, rightObjectProperties, JsonDiffHelpers.JsonPathCombineWithArrayIndex),
            MatchJsonObjectPropertiesBy.Name => EnumerateElementsDifferencesByKey(jsonPath, leftObjectProperties, rightObjectProperties, JsonDiffHelpers.JsonPathCombinePropertyName),
            _ => throw new NotImplementedException($"Matching object properties by '{ObjectPropertiesMatchingStrategy}' is not (yet) implemented."),
        };

    private IEnumerable<JsonDifference<TNode>> EnumerateElementsDifferencesByPosition(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftElements,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightElements,
        Func<string, int, string> jsonPathCombineSelector)
    {
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> leftArrayEnumerator = leftElements.GetEnumerator();
        using IEnumerator<JsonDiffArrayElementDescriptor<TNode>> rightArrayEnumerator = rightElements.GetEnumerator();

        while (true)
        {
            bool leftArrayHasItems = leftArrayEnumerator.MoveNext();
            bool rightArrayHasItems = rightArrayEnumerator.MoveNext();

            if (leftArrayHasItems && rightArrayHasItems)
            {
                string arrayItemJsonPath = jsonPathCombineSelector(jsonPath, leftArrayEnumerator.Current.Index);
                IEnumerable<JsonDifference<TNode>> differencesOnThisArrayItem = EnumerateDifferences(arrayItemJsonPath, leftArrayEnumerator.Current.ArrayElement, rightArrayEnumerator.Current.ArrayElement);
                foreach (var difference in differencesOnThisArrayItem)
                {
                    yield return difference;
                }
            }
            else if (leftArrayHasItems)
            {
                string arrayItemJsonPath = jsonPathCombineSelector(jsonPath, leftArrayEnumerator.Current.Index);
                yield return new JsonDifference<TNode>(arrayItemJsonPath, JsonDifferenceSide.Left, leftArrayEnumerator.Current.ArrayElement);
            }
            else if (rightArrayHasItems)
            {
                string arrayItemJsonPath = jsonPathCombineSelector(jsonPath, rightArrayEnumerator.Current.Index);
                yield return new JsonDifference<TNode>(arrayItemJsonPath, JsonDifferenceSide.Right, rightArrayEnumerator.Current.ArrayElement);
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerable<JsonDifference<TNode>> EnumerateElementsDifferencesByKey(
        string jsonPath,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> leftElements,
        IEnumerable<JsonDiffArrayElementDescriptor<TNode>> rightElements,
        Func<string, string, string> jsonPathCombineSelector)
    {
        var leftElementsGroupedByKey = leftElements
            .ToLookup(
                keySelector: element => element.Key,
                elementSelector: element => element.ArrayElement
            );

        var rightElementsGroupedByKey = rightElements
            .ToLookup(
                keySelector: element => element.Key,
                elementSelector: element => element.ArrayElement
            );

        var leftExtraElements = leftElements
            .Where(element => !rightElementsGroupedByKey.Contains(element.Key));

        foreach (var element in leftExtraElements)
        {
            string elementJsonPath = jsonPathCombineSelector(jsonPath, element.Key);
            yield return new JsonDifference<TNode>(elementJsonPath, JsonDifferenceSide.Left, element.ArrayElement);
        }

        var rightExtraElements = rightElements
            .Where(element => !leftElementsGroupedByKey.Contains(element.Key));

        foreach (var element in rightExtraElements)
        {
            string elementJsonPath = jsonPathCombineSelector(jsonPath, element.Key);
            yield return new JsonDifference<TNode>(elementJsonPath, JsonDifferenceSide.Right, element.ArrayElement);
        }

        var leftAndRightJoinedByKey = leftElementsGroupedByKey
            .Join(
                inner: rightElementsGroupedByKey,
                outerKeySelector: left => left.Key,
                innerKeySelector: right => right.Key,
                resultSelector: (left, right) => new ValueTuple<string, IEnumerable<TNode?>, IEnumerable<TNode?>>(left.Key, left, right)
            );

        foreach (var joined in leftAndRightJoinedByKey)
        {
            string elementJsonPath = jsonPathCombineSelector(jsonPath, joined.Item1);

            var leftElementValues = joined.Item2
                .Select((value, index) => new JsonDiffArrayElementDescriptor<TNode>(index, joined.Item1, value))
                .OrderBy(kvp => kvp.Key)
                .ToList();

            var rightElementValues = joined.Item3
                .Select((value, index) => new JsonDiffArrayElementDescriptor<TNode>(index, joined.Item1, value))
                .OrderBy(kvp => kvp.Key)
                .ToList();

            IEnumerable<JsonDifference<TNode>> differences = leftElementValues.Count > 1 || rightElementValues.Count > 1
                ? EnumerateElementsDifferencesByPosition(elementJsonPath, leftElementValues, rightElementValues, JsonDiffHelpers.JsonPathCombineWithArrayIndex)
                : EnumerateDifferences(elementJsonPath, leftElementValues[0].ArrayElement, rightElementValues[0].ArrayElement);

            foreach (var difference in differences)
            {
                yield return difference;
            }
        }
    }
}

public enum MatchJsonArrayElementsBy
{
    Position,
    Key
}

public enum MatchJsonObjectPropertiesBy
{
    Name,
    Position
}
