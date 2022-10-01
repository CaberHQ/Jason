namespace Caber.Jason.Client;

public sealed class JsonRpcWebSocketOptions
{
    public Uri Uri { get; set; }
    
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(20);
}