namespace NoP77svk.JsonDiff;

using System.Collections.Generic;
using System.Text.Json;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record JsonDiffArrayElementDescriptor<TNode>(int Index, string Key, TNode? ArrayElement);
#pragma warning restore SA1313

public interface IJsonDiffNodeValuesSelector<TNode>
{
    JsonValueKind GetValueKind(TNode? node);

    string GetStringValue(TNode? node);

    decimal GetNumberValue(TNode? node);

    IEnumerable<JsonDiffArrayElementDescriptor<TNode>> GetArrayValues(TNode? node);

    string GetArrayElementKey(int index, TNode? node);

    IEnumerable<JsonDiffArrayElementDescriptor<TNode>> GetObjectProperties(TNode? node);
}
