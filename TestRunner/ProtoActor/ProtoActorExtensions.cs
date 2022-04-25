using TestRunner.Tests;

namespace TestRunner.ProtoActor;

public static class ProtoActorExtensions
{
    public static WebApplicationBuilder AddProtoActorTestServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ProtoActorTestServices>();
        builder.Services.AddSingleton<Ping>(provider => provider.GetRequiredService<ProtoActorTestServices>().Ping);
        builder.Services.AddSingleton<Activate>(provider => provider.GetRequiredService<ProtoActorTestServices>().Activate);

        return builder;
    }
}