namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Text.Json;

public interface IJsonDiffNodeValuesSelector<TNode>
{
    JsonValueKind GetValueKind(TNode node);
    string GetStringValue(TNode node);
    decimal GetNumberValue(TNode node);
    IEnumerable<KeyValuePair<int, TNode>> GetArrayValues(TNode node);
    IEnumerable<KeyValuePair<string, TNode>> GetObjectProperties(TNode node);
}
