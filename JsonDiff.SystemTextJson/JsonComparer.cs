namespace NoP77svk.JsonDiff;

using System.Text.Json;
using System.Text.Json.Nodes;

public class JsonElementComparer : JsonComparer<JsonElement>
{
    public JsonElementComparer()
        : base(JsonElementDiffValuesSelector.DefaultInstance)
    {
    }
}

public class JsonNodeComparer : JsonComparer<JsonNode?>
{
    public JsonNodeComparer()
        : base(JsonNodeDiffValuesSelector.DefaultInstance)
    {
    }
}
