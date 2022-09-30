namespace Caber.Jason.Core;

public interface IMessageFormatter
{
    Task SerializeAsync<T>(Stream stream, T value);
    Task<T> DeserializeAsync<T>(Stream stream);
}