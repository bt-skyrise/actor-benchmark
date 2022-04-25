using TestRunner.Tests;

namespace TestRunner.Akka;

public static class AkkaExtensions
{
    public static WebApplicationBuilder AddAkkaClusterProxyHostedService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<ClusterProxyHostedService>();
        return builder;
    }

    public static WebApplicationBuilder AddAkkaTestServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<AkkaTestServices>();
        builder.Services.AddSingleton<Ping>(ctx => ctx.GetRequiredService<AkkaTestServices>().Ping);
        builder.Services.AddSingleton<Activate>(ctx => ctx.GetRequiredService<AkkaTestServices>().Activate);

        return builder;
    }
}