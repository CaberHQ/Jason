using Caber.Jason.Core;
using Caber.Jason.Serializer.MessagePack.Formatters;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Caber.Jason.Serializer.MessagePack;

public class MessagePackMessageSerializer : IMessageSerializer
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(new JsonRpcParamsFormatter()));

    public async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        await MessagePackSerializer.SerializeAsync<T>(stream, value, cancellationToken: cancellationToken);
    }

    public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return await MessagePackSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
    }
}