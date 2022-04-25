using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansSut.Shared;
using TestRunner.Tests;

namespace TestRunner.Orleans;

public static class OrleansExtensions
{
    public static async Task AddOrleansClient(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection("Orleans");
        
        var orleansClient = new ClientBuilder()
            .Configure<ClusterOptions>(opt =>
            {
                opt.ClusterId = config["ClusterId"];
                opt.ServiceId = config["ServiceId"];
            })
            .UseAzureStorageClustering(opt =>
            {
                opt.ConfigureTableServiceClient(config["ClusteringStorage"]);
            })
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IPingPongGrain).Assembly))
            .Build();

        await orleansClient.Connect();

        builder.Services.AddSingleton(orleansClient);

        builder.Services.AddSingleton<OrleansTestServices>();
        builder.Services.AddSingleton<Ping>(ctx => ctx.GetRequiredService<OrleansTestServices>().Ping);
        builder.Services.AddSingleton<Activate>(ctx => ctx.GetRequiredService<OrleansTestServices>().Activate);
    }
}