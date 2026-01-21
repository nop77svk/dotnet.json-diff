namespace NoP77svk.JsonCompare;
#pragma warning disable SA1313

public record JsonDifference<TNode>(string NodePath, JsonDifferenceSide Side, TNode? NodeValue);

public enum JsonDifferenceSide
{
    Left,
    Right
}
