using System.Text.Json.Serialization;
using Caber.Jason.Core.Converters;

namespace Caber.Jason.Core.Model;

[JsonConverter(typeof(JsonRpcParamsConverter))]
public class JsonRpcParams
{
    public JsonRpcParamsType Type { get; }

    public IDictionary<string, object> ByNameValue { get; }
    
    public IEnumerable<object> ByPositionValue { get; }

    public JsonRpcParams(IDictionary<string, object> value)
    {
        Type = JsonRpcParamsType.ByName;
        ByNameValue = value;
    }

    public JsonRpcParams(IEnumerable<object> value)
    {
        Type = JsonRpcParamsType.ByPosition;
        ByPositionValue = value;
    }
}