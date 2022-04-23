namespace RpcServer.Api;

public interface IHttpClientFactory
{
    public HttpClient CreateHttpClient();
}