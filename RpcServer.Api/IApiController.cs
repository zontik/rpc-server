namespace RpcServer.Api;

public interface IApiController
{
    Task VoidMethod(string param, CancellationToken ct);
    Task<string> ResultMethod(string param, CancellationToken ct);
}