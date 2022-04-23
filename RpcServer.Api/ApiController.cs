using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace RpcServer.Api;

public class ApiController : IApiController
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _serializerSettings;

    public ApiController(HttpClient httpClient, JsonSerializerSettings serializerSettings)
    {
        _httpClient = httpClient;
        _serializerSettings = serializerSettings;
    }

    #region IApiController
    Task IApiController.VoidMethod(string param, CancellationToken ct)
    {
        return ExecuteVoidCommand(nameof(IApiController.VoidMethod), ct, param);
    }

    Task<string> IApiController.ResultMethod(string param, CancellationToken ct)
    {
        return ExecuteCommand<string>(nameof(IApiController.ResultMethod), ct, param);
    }
    #endregion IApiController

    private async Task<R> ExecuteCommand<R>(string commandName, CancellationToken ct, params object[] args)
    {
        var arguments = JsonConvert.SerializeObject(args, Formatting.None, _serializerSettings);
        var content = new StringContent(arguments, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var response = await _httpClient.PostAsync(MakeRoutePath(commandName), content, ct);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<R>(responseContent, _serializerSettings)!;
    }

    private async Task ExecuteVoidCommand(string commandName, CancellationToken ct, params object[] args)
    {
        var arguments = JsonConvert.SerializeObject(args, Formatting.None, _serializerSettings);
        var content = new StringContent(arguments, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var response = await _httpClient.PostAsync(MakeRoutePath(commandName), content, ct);
        response.EnsureSuccessStatusCode();
    }

    private static string MakeRoutePath(string commandName)
    {
        return $"/api/command/{commandName}";
    }
}