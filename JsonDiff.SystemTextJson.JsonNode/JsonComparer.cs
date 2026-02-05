namespace NoP77svk.JsonDiff;

using System.Text.Json.Nodes;

public class JsonNodeComparer : JsonComparer<JsonNode?>
{
    public JsonNodeComparer()
        : base(JsonNodeDiffValuesSelector.DefaultInstance)
    {
    }
}
