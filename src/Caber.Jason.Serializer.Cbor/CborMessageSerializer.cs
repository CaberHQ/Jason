using Caber.Jason.Core;
using Caber.Jason.Core.Model;
using Caber.Jason.Serializer.Cbor.Converters;
using Dahomey.Cbor;
using JsonRpcParamsConverter = Caber.Jason.Serializer.Cbor.Converters.JsonRpcParamsConverter;

namespace Caber.Jason.Serializer.Cbor;

public class CborMessageSerializer : IMessageSerializer
{
    private static readonly CborOptions _options = new();

    static CborMessageSerializer()
    {
        _options.Registry.ConverterRegistry.RegisterConverter(typeof(JsonRpcParams), new Converters.JsonRpcParamsConverter());
    }
    
    public async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        await Dahomey.Cbor.Cbor.SerializeAsync(value, stream, _options, cancellationToken);
    }

    public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return await Dahomey.Cbor.Cbor.DeserializeAsync<T>(stream, _options, cancellationToken);
    }
}