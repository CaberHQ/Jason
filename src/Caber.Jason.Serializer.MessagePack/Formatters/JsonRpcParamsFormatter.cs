using Caber.Jason.Core.Model;
using MessagePack;
using MessagePack.Formatters;

namespace Caber.Jason.Serializer.MessagePack.Formatters;

public class JsonRpcParamsFormatter : IMessagePackFormatter<JsonRpcParams>
{
    public void Serialize(ref MessagePackWriter writer, JsonRpcParams value, MessagePackSerializerOptions options)
    {
        if (value.Type == JsonRpcParamsType.ByPosition)
        {
            options.Resolver.GetFormatterWithVerify<IEnumerable<object>>()
                .Serialize(ref writer, value.ByPositionValue, options);
        }
        else if (value.Type == JsonRpcParamsType.ByName)
        {
            options.Resolver.GetFormatter<IDictionary<string, object>>()
                .Serialize(ref writer, value.ByNameValue, options);
        }
        else
        {
            throw new InvalidOperationException($"Cannot serialize '{nameof(JsonRpcParams)}' invalid type");
        }
    }

    public JsonRpcParams Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.NextMessagePackType == MessagePackType.Array)
        {
            var array = options.Resolver.GetFormatterWithVerify<IEnumerable<object>>()
                .Deserialize(ref reader, options);

            return new JsonRpcParams(array);
        }
        else if (reader.NextMessagePackType == MessagePackType.Map)
        {
            var map = options.Resolver.GetFormatterWithVerify<IDictionary<string, object>>()
                .Deserialize(ref reader, options);

            return new JsonRpcParams(map);
        }
        else
        {
            throw new InvalidOperationException($"Cannot deserialize '{nameof(JsonRpcParams)}' when type is '{reader.NextMessagePackType}', expecting '{MessagePackType.Array}' or '{MessagePackType.Map}'");
        }
    }
}