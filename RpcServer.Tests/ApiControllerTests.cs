using System;
using System.Threading.Tasks;
using Xunit;

namespace RpcServer.Tests;

public class ApiControllerTests : ControllerTestsBase
{
    [Fact]
    [Trait("Category", "Common")]
    public async Task ResultMethod_ReturnsResult()
    {
        //Arrange
        //Arrange

        //Act
        var param = Guid.NewGuid().ToString();
        var result = await ApiClient.ApiController.ResultMethod(param, default);
        //Act

        //Assert
        Assert.Equal(param, result);
        //Assert
    }

    [Fact]
    [Trait("Category", "Common")]
    public async Task VoidMethod_ExecutesOk()
    {
        //Arrange
        //Arrange

        //Act
        var param = Guid.NewGuid().ToString();
        await ApiClient.ApiController.VoidMethod(param, default);
        //Act

        //Assert
        //Assert
    }
}