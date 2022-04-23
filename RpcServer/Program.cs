using RpcServer.Services;

namespace RpcServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<SerializationService>();
        builder.Services.AddControllers().AddNewtonsoftJson();

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}