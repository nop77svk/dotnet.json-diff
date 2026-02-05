namespace NoP77svk.JsonDiff;

using System.Text.Json;

internal static class Helpers
{
    internal static JsonDiffValueKind ToInternalValueKind(this JsonValueKind valueKind)
        => valueKind switch
        {
            JsonValueKind.Undefined => JsonDiffValueKind.Unknown,
            JsonValueKind.Object => JsonDiffValueKind.Object,
            JsonValueKind.Array => JsonDiffValueKind.Array,
            JsonValueKind.String => JsonDiffValueKind.String,
            JsonValueKind.Number => JsonDiffValueKind.Number,
            JsonValueKind.True => JsonDiffValueKind.Boolean,
            JsonValueKind.False => JsonDiffValueKind.Boolean,
            JsonValueKind.Null => JsonDiffValueKind.Null,
            _ => throw new ArgumentOutOfRangeException(nameof(valueKind), valueKind.ToString())
        };
}
