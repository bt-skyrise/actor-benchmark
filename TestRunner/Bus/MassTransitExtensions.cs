using System.Reflection;
using MassTransit;

namespace TestRunner.Bus;

public static class MassTransitExtensions
{
    public static void AddMassTransit(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection("Bus");
        var endpoint = Environment.MachineName;

        builder.Services.AddMassTransit(busCfg =>
        {
            busCfg.UsingAzureServiceBus((ctx, cfg) =>
            {
                cfg.Host(config["ServiceBusConnectionString"]);
                
                cfg.ReceiveEndpoint(endpoint, e =>
                {
                    e.AutoDeleteOnIdle = TimeSpan.FromMinutes(10);
                    e.RemoveSubscriptions = true;
                    
                    e.ConfigureConsumer<RunTestConsumer>(ctx);
                });
            });
            
            busCfg.AddConsumers(Assembly.GetExecutingAssembly());
        });
    }
}