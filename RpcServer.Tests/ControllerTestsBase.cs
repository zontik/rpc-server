using System;
using Microsoft.AspNetCore.Mvc.Testing;
using RpcServer.Api;

namespace RpcServer.Tests;

public class ControllerTestsBase
{
    protected readonly ApiClient ApiClient;

    protected ControllerTestsBase()
    {
        var application = new WebApplicationFactory<Program>();
        var httpClientFactory = new TestsHttpClientFactory(application.Server.CreateHandler());
        ApiClient = new ApiClient(new Uri("http://localhost/api/"), httpClientFactory);
    }
}
