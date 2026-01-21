namespace NoP77svk.JsonCompare;

using System.Collections.Generic;
using System.Linq;

public interface IJsonDiffFormatter<TNode>
{
    string DiffMessageFormatter(JsonDiff.Difference<TNode> difference);
}

public class SimpleJsonDiffFormatter<TNode>
    : IJsonDiffFormatter<TNode>
{
    public string LeftSideChangeDescription { get; init; } = @"[+] Extra in left/missig in right";
    public string RightSideChangeDescription { get; init; } = @"[-] Missing in left/extra in right";
    public string UnknownSideChangeDescription { get; init; } = @"?";

    public string DiffMessageFormatter(JsonDiff.Difference<TNode> difference)
    {
        string diffIndicator = difference.Side switch
        {
            JsonDiff.DifferenceSide.Left => LeftSideChangeDescription,
            JsonDiff.DifferenceSide.Right => RightSideChangeDescription,
            _ => UnknownSideChangeDescription,
        };

        string differenceDisplay = $"{diffIndicator}\n{difference.NodePath}: {difference.NodeValue}";

        return differenceDisplay;
    }
}

public static class JsonDiffFormatterExtensions
{
    public static IEnumerable<string> AsFormattedStrings<TNode>(this IEnumerable<JsonDiff.Difference<TNode>> differences, IJsonDiffFormatter<TNode> options)
        => differences.Select(options.DiffMessageFormatter);
}
