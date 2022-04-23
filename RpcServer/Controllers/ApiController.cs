using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RpcServer.Api;
using RpcServer.Services;

namespace RpcServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase, IApiController
{
    private readonly SerializationService _serializationService;
    private readonly ILogger<ApiController> _logger;

    public ApiController(SerializationService serializationService, ILogger<ApiController> logger)
    {
        _serializationService = serializationService;
        _logger = logger;
    }

    [HttpPost("command/{commandName}")]
    public async Task<ActionResult> ProcessCommand(string commandName, CancellationToken ct)
    {
        try
        {
            using var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8);
            var callArgs = await reader.ReadToEndAsync();
            var args = JArray.Parse(callArgs);

            var result = await RpcRouter<IApiController>.Router.ExecuteAction(this, commandName, args, _serializationService.Serializer, ct);

            var resultString = JsonConvert.SerializeObject(result, _serializationService.Settings);
            return Content(resultString, MediaTypeNames.Application.Json, System.Text.Encoding.UTF8);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, string.Empty);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    #region IApiController
    Task IApiController.VoidMethod(string param, CancellationToken token)
    {
        _logger.LogInformation($"{nameof(IApiController.VoidMethod)} param: {param}");
        return Task.CompletedTask;
    }

    Task<string> IApiController.ResultMethod(string param, CancellationToken token)
    {
        _logger.LogInformation($"{nameof(IApiController.ResultMethod)} param: {param}");
        return Task.FromResult(param);
    }
    #endregion IApiController
}