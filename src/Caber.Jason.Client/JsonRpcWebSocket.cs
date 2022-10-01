using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Text.Json;
using Caber.Jason.Core;
using Caber.Jason.Core.Model;

namespace Caber.Jason.Client;

public sealed class JsonRpcWebSocket : IDisposable
{
    private readonly Uri _uri;

    private readonly ClientWebSocket _socket;

    private readonly TimeSpan _timeout;

    private readonly IMessageSerializer _messageSerializer;

    public JsonRpcWebSocket(JsonRpcWebSocketOptions options)
    {
        _uri = options.Uri;
        _timeout = options.DefaultTimeout;
        _messageSerializer = options.MessageSerializer;

        _socket = new ClientWebSocket();
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _socket.ConnectAsync(_uri, cancellationToken);
    }

    public async Task<JsonRpcResponse<T>> CallAsync<T>(string method, IDictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        var request = new JsonRpcRequest
        {
            Id = Guid.NewGuid().ToString(),
            Method = method,
            Params = new JsonRpcParams(parameters),
        };

        return await SendAsync<T>(request, cancellationToken);
    }
    
    public async Task<JsonRpcResponse<T>> CallAsync<T>(string method, IEnumerable<object> parameters,
        CancellationToken cancellationToken = default)
    {
        var request = new JsonRpcRequest
        {
            Id = Guid.NewGuid().ToString(),
            Method = method,
            Params = new JsonRpcParams(parameters),
        };

        return await SendAsync<T>(request, cancellationToken);
    }

    public async Task<JsonRpcResponse<T>> SendAsync<T>(JsonRpcRequest request,
        CancellationToken cancellationToken = default) where T : notnull
    {
        using var timeoutTokenSource = new CancellationTokenSource(_timeout);

        using var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(timeoutTokenSource.Token, cancellationToken);

        await HandleSendAsync<T>(request, linkedTokenSource.Token);

        return await HandleReceiveAsync<T>(linkedTokenSource.Token);
    }

    private async Task HandleSendAsync<T>(JsonRpcRequest request, CancellationToken cancellationToken) where T : notnull
    {
        var sendPipe = new Pipe();

        var serializeTask = SerializeAsync(sendPipe.Writer, request, cancellationToken);
        var sendTask = SendOverSocketAsync(sendPipe.Reader, cancellationToken);

        await Task.WhenAll(serializeTask, sendTask);
    }

    private async Task<JsonRpcResponse<T>> HandleReceiveAsync<T>(CancellationToken cancellationToken) where T : notnull
    {
        var receivePipe = new Pipe();

        var receiveTask = ReceiveOverSocketAsync(receivePipe.Writer, cancellationToken);
        var deserializeTask = DeserializeAsync<T>(receivePipe.Reader, cancellationToken);

        await Task.WhenAll(receiveTask, deserializeTask);

        return deserializeTask.Result;
    }

    private async Task SendOverSocketAsync(PipeReader reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);

            var buffer = result.Buffer;

            foreach (var memory in buffer)
            {
                await _socket.SendAsync(memory, WebSocketMessageType.Text, WebSocketMessageFlags.None,
                    cancellationToken);
            }

            reader.AdvanceTo(buffer.End);

            if (result.IsCompleted)
                break;
        }

        await _socket.SendAsync(ReadOnlyMemory<byte>.Empty, WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage, cancellationToken);

        await reader.CompleteAsync();
    }

    private async Task ReceiveOverSocketAsync(PipeWriter writer, CancellationToken cancellationToken)
    {
        while (true)
        {
            var memory = writer.GetMemory();

            var result = await _socket.ReceiveAsync(memory, cancellationToken);

            if (result.Count > 0)
            {
                var slice = memory.Slice(0, result.Count);

                await writer.WriteAsync(slice, cancellationToken);
            }

            if (result.EndOfMessage)
                break;
        }

        await writer.FlushAsync(cancellationToken);

        await writer.CompleteAsync();
    }

    private async Task SerializeAsync(PipeWriter writer, JsonRpcRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = writer.AsStream(true);

            await _messageSerializer.SerializeAsync(stream, request, cancellationToken);
        }
        finally
        {
            await writer.CompleteAsync();
        }
    }

    private async Task<JsonRpcResponse<T>?> DeserializeAsync<T>(PipeReader reader, CancellationToken cancellationToken)
        where T : notnull
    {
        try
        {
            await using var stream = reader.AsStream(true);

            return await _messageSerializer.DeserializeAsync<JsonRpcResponse<T>?>(stream, cancellationToken);
        }
        finally
        {
            await reader.CompleteAsync();
        }
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}