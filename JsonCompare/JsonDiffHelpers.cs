namespace NoP77svk.JsonDiff;

using System.Text.RegularExpressions;

internal static class JsonDiffHelpers
{
    private static readonly Regex _rxPropertyNameIsClean = new Regex(@"^[a-z_][a-z0-9_]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string JsonPathCombine(string jsonPath, string propertyName)
    {
        string sanitisedPropertyName = SanitisePropertyName(propertyName);
        return $"{jsonPath}.{sanitisedPropertyName}";
    }

    private static string SanitisePropertyName(string propertyName)
        => _rxPropertyNameIsClean.IsMatch(propertyName)
        ? propertyName
        : $"\"{propertyName}\"";
}
