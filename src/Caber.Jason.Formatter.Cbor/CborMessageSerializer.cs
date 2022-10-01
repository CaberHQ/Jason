using Caber.Jason.Core;

namespace Caber.Jason.Formatter.Cbor;

public class CborMessageSerializer : IMessageSerializer
{
    public async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        await Dahomey.Cbor.Cbor.SerializeAsync(value, stream, token: cancellationToken);
    }

    public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return await Dahomey.Cbor.Cbor.DeserializeAsync<T>(stream, token: cancellationToken);
    }
}