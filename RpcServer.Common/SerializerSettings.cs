using Newtonsoft.Json;

namespace RpcServer.Common;

public static class SerializerSettings
{
    public static JsonSerializerSettings Create()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
    }
}