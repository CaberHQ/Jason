using Caber.Jason.Core.Model;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Converters;

namespace Caber.Jason.Formatter.Cbor.Converters;

public class JsonRpcParamsConverter : CborConverterBase<JsonRpcParams>
{
    public override JsonRpcParams Read(ref CborReader reader)
    {
        throw new NotImplementedException();
    }

    public override void Write(ref CborWriter writer, JsonRpcParams value)
    {
        throw new NotImplementedException();
    }
}