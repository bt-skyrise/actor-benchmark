using System.Reflection;
using OrleansSut.Server;
using OrleansSut.Shared;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Host.UseSerilog((_, lcfg) =>
        lcfg
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Seq(builder.Configuration["SeqUrl"])
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Service", Assembly.GetExecutingAssembly().GetName().Name));

    builder.AddOrleans(typeof(PingPongGrain).Assembly);
    
    var app = builder.Build();

    app.Run();
}
catch (Exception e)
{
    Log.Logger.Fatal(e, "Service crash");
    throw;
}
finally
{
    Log.CloseAndFlush();
}