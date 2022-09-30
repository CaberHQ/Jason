using System.Text.Json.Serialization;

namespace Caber.Jason.Core.Model;

public abstract class JsonRpcObjectBase
{
    [JsonPropertyName("jsonrpc")]
    public string Version { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public string Id { get; set; }
}