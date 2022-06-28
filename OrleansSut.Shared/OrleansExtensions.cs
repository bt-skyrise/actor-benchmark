using System.Reflection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace OrleansSut.Shared;

public static class OrleansExtensions
{
    public static WebApplicationBuilder AddOrleans(this WebApplicationBuilder builder, params Assembly[] applicationParts)
    {
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
                .UseAzureStorageClustering(options => options.ConfigureTableServiceClient(config["ClusteringStorage"]));

            if (applicationParts.Any())
            {
                siloBuilder.ConfigureApplicationParts(parts =>
                {
                    foreach (var applicationPart in applicationParts)
                        parts.AddApplicationPart(applicationPart).WithReferences();
                });
            }
        });

        return builder;
    }
}