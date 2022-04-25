using System.Reflection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansSut.Server;
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

    builder.Host.UseOrleans(siloBuilder =>
    {
        var config = builder.Configuration.GetSection("Orleans");

        if (builder.Environment.IsDevelopment())
        {
            siloBuilder
                .Configure<ClusterOptions>(opt =>
                {
                    opt.ClusterId = config["ClusterId"];
                    opt.ServiceId = config["ServiceId"];
                })
                .ConfigureEndpoints(
                    config.GetValue("SiloPort", 11111),
                    config.GetValue("GatewayPort", 30000)
                );
        }
        else
        {
            siloBuilder.UseKubernetesHosting();
        }
        
        siloBuilder
            .UseAzureStorageClustering(options => options.ConfigureTableServiceClient(config["ClusteringStorage"]))
            .ConfigureApplicationParts(parts =>
                parts.AddApplicationPart(typeof(PingPongGrain).Assembly).WithReferences());
    });

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