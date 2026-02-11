namespace NoP77svk.JsonDiff;

public static class JsonComparerExtensions
{
    /// <summary>
    /// Enumerates the differences between two (artbitrary type) JSON documents/nodes.
    /// </summary>
    /// <param name="leftDocument">Left JSON document/node.</param>
    /// <param name="rightDocument">Right JSON document/node.</param>
    /// <returns>
    ///     <para>An enumerable with differences, one for extra/changed value from the left side, one for extra/changed value from the right side.</para>
    ///     <para>If there's only a "left" side node returned, it means there was no matching "right" side node.</para>
    ///     <para>If there's only a "right" side node returned, it means there was no matching "left" side node.</para>
    /// </returns>
    public static IEnumerable<JsonDifference<TNode>> CompareWith<TNode>(this TNode? leftDocument, TNode? rightDocument, IJsonDiffNodeValuesSelector<TNode> nodeValuesSelector)
        => new JsonComparer<TNode>(nodeValuesSelector).EnumerateDifferences(leftDocument, rightDocument);
}
