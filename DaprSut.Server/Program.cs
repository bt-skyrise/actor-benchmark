using System.Reflection;
using DaprSut.Server;
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

    builder.Services.AddActors(opt => opt.Actors.RegisterActor<PingPongActor>());
    
    var app = builder.Build();

    app.MapActorsHandlers();

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