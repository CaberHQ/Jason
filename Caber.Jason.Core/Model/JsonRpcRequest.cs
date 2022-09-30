using System.Text.Json.Serialization;

namespace Caber.Jason.Core.Model;

public class JsonRpcRequest : JsonRpcObjectBase
{
    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("params")]
    public JsonRpcParams Params { get; set; }
}