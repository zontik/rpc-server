using System.Net.Http.Headers;
using System.Net.Mime;
using RpcServer.Common;

namespace RpcServer.Api;

public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public IApiController ApiController { get; }

    public ApiClient(Uri url, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateHttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
        _httpClient.BaseAddress = url;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true };

        ApiController = new ApiController(_httpClient, SerializerSettings.Create());
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}