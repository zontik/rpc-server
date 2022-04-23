using System;
using System.Net.Http;

namespace RpcServer.Tests;

public class TestsHttpClientFactory : Api.IHttpClientFactory, IDisposable
{
    private readonly HttpMessageHandler _httpMessageHandler;

    public TestsHttpClientFactory(HttpMessageHandler httpMessageHandler = default)
    {
        _httpMessageHandler = httpMessageHandler ?? new HttpClientHandler();
    }

    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }

    public HttpClient CreateHttpClient()
    {
        return new HttpClient(_httpMessageHandler, false);
    }
}