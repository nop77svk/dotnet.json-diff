namespace NoP77svk.JsonDiff;

using System.Text.Json;

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
    public static IEnumerable<JsonDifference<JsonElement>> CompareWith(this JsonDocument leftDocument, JsonDocument rightDocument)
        => leftDocument.RootElement.CompareWith(rightDocument.RootElement);

    /// <summary>
    /// Enumerates the differences between two (artbitrary type) JSON documents/nodes.
    /// </summary>
    /// <param name="leftElement">Left JSON document/node.</param>
    /// <param name="rightElement">Right JSON document/node.</param>
    /// <returns>
    ///     <para>An enumerable with differences, one for extra/changed value from the left side, one for extra/changed value from the right side.</para>
    ///     <para>If there's only a "left" side node returned, it means there was no matching "right" side node.</para>
    ///     <para>If there's only a "right" side node returned, it means there was no matching "left" side node.</para>
    /// </returns>
    public static IEnumerable<JsonDifference<JsonElement>> CompareWith(this JsonElement leftElement, JsonElement rightElement)
        => leftElement.CompareWith(rightElement, JsonElementDiffValuesSelector.DefaultInstance);
}
