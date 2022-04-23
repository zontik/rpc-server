using Newtonsoft.Json;
using RpcServer.Common;

namespace RpcServer.Services;

public class SerializationService
{
    public readonly JsonSerializerSettings Settings;
    public readonly JsonSerializer Serializer;

    public SerializationService()
    {
        Settings = SerializerSettings.Create();
        Serializer = JsonSerializer.Create(Settings);
    }
}