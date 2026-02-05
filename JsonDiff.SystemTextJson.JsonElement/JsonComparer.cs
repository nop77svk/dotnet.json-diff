namespace NoP77svk.JsonDiff;

using System.Text.Json;

public class JsonElementComparer : JsonComparer<JsonElement>
{
    public JsonElementComparer()
        : base(JsonElementDiffValuesSelector.DefaultInstance)
    {
    }
}
