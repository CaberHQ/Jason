using System.Text.Json.Serialization;

namespace Caber.Jason.Core.Model;

public class JsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}