using System.Text.Json;
using Caber.Jason.Core.Model;

namespace Caber.Jason.Core;

public class DefaultMessageSerializer : IMessageSerializer
{
    public async Task SerializeAsync<T>(Stream stream, T value,
        CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);
    }

    public async Task<T> DeserializeAsync<T>(Stream stream,
        CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
    }
}