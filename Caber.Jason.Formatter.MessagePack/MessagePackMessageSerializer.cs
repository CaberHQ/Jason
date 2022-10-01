using Caber.Jason.Core;
using MessagePack;

namespace Caber.Jason.Formatter.MessagePack;

public class MessagePackMessageSerializer : IMessageSerializer
{
    public async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        await MessagePackSerializer.SerializeAsync<T>(stream, value, cancellationToken: cancellationToken);
    }

    public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return await MessagePackSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
    }
}