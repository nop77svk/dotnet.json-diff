#pragma warning disable S101 // Types should be named in PascalCase - this is a static class with only properties, so it doesn't need to be instantiated, and the name is descriptive enough.
namespace NoP77svk.JsonDiff.Tests;

using System.Text;
using System.Text.RegularExpressions;

public static class JsonDiff_Data
{
    private const string ShuffledFileSuffix = "shuffled";
    private const string ShuffledKvpFileSuffix = "kvp-shuffled";

    public static IEnumerable<string> BasicTestCases => Directory.EnumerateFiles(nameof(JsonDiff_Data), "*.json", SearchOption.AllDirectories)
        .Where(fname => !fname.EndsWith($".{ShuffledFileSuffix}.json", StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<ShuffledJsonTestCase> ShuffledTestCases => Directory.EnumerateFiles(nameof(JsonDiff_Data), $"*.{ShuffledFileSuffix}.json", SearchOption.AllDirectories)
        .Select(fname => new ShuffledJsonTestCase(_rxRemoveShuffledSuffix.Replace(fname, ".json"), fname));

    public static IEnumerable<ShuffledJsonTestCase> ShuffledKvpTestCases => Directory.EnumerateFiles(nameof(JsonDiff_Data), $"*.{ShuffledKvpFileSuffix}.json", SearchOption.AllDirectories)
        .Select(fname => new ShuffledJsonTestCase(_rxRemoveShuffledKvpSuffix.Replace(fname, ".json"), fname));

    public static string DifferencesToString<TNode>(IEnumerable<JsonDifference<TNode>> differences, IJsonDiffFormatter<TNode> diffFormatter)
    {
        StringBuilder result = new();
        result.AppendLine("Differences:");
        result.AppendLine("------------------------------------------------------------------------------");

        foreach (string diff in differences.AsFormattedStrings(diffFormatter))
        {
            result.AppendLine(diff);
        }

        result.AppendLine("------------------------------------------------------------------------------");

        return result.ToString();
    }

    private static readonly Regex _rxRemoveShuffledSuffix = new Regex(@$"\.{ShuffledFileSuffix}\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex _rxRemoveShuffledKvpSuffix = new Regex(@$"\.{ShuffledKvpFileSuffix}\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}

#pragma warning disable SA1313
public record ShuffledJsonTestCase(string OriginalFileName, string ShuffledFileName);
#pragma warning restore SA1313
