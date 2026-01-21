namespace NoP77svk.JsonDiff;

using System.Collections.Generic;
using System.Linq;

public interface IJsonDiffFormatter<TNode>
{
    string DiffMessageFormatter(JsonDifference<TNode> difference);
}

public class SimpleJsonDiffFormatter<TNode>
    : IJsonDiffFormatter<TNode>
{
    public string LeftSideChangeDescription { get; init; } = @"[+] Extra in left/missig in right";
    public string RightSideChangeDescription { get; init; } = @"[-] Missing in left/extra in right";
    public string UnknownSideChangeDescription { get; init; } = @"?";

    public string DiffMessageFormatter(JsonDifference<TNode> difference)
    {
        string diffIndicator = difference.Side switch
        {
            JsonDifferenceSide.Left => LeftSideChangeDescription,
            JsonDifferenceSide.Right => RightSideChangeDescription,
            _ => UnknownSideChangeDescription,
        };

        string differenceDisplay = $"{diffIndicator}\n{difference.NodePath}: {difference.NodeValue}";

        return differenceDisplay;
    }
}

public static class JsonDiffFormatterExtensions
{
    public static IEnumerable<string> AsFormattedStrings<TNode>(this IEnumerable<JsonDifference<TNode>> differences, IJsonDiffFormatter<TNode> options)
        => differences.Select(options.DiffMessageFormatter);
}
