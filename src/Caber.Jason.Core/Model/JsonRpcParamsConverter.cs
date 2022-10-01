using System.Text.Json;
using System.Text.Json.Serialization;

namespace Caber.Jason.Core.Model;

public class JsonRpcParamsConverter : JsonConverter<JsonRpcParams>
{
    public override JsonRpcParams? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.StartArray)
        {
            var array = JsonSerializer.Deserialize<IEnumerable<object>>(ref reader);

            return new JsonRpcParams(array);
        }
        else if (reader.TokenType is JsonTokenType.StartObject)
        {
            var map = JsonSerializer.Deserialize<IDictionary<string, object>>(ref reader);

            return new JsonRpcParams(map);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, JsonRpcParams value, JsonSerializerOptions options)
    {
        if (value.Type is JsonRpcParamsType.ByPosition)
        {
            writer.WriteStartArray();

            foreach (var item in value.ByPositionValue)
            {
                writer.WriteRawValue(JsonSerializer.Serialize(item));
            }

            writer.WriteEndArray();
        }
    }
}