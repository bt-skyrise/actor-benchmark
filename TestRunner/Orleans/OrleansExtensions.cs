using TestRunner.Tests;

namespace TestRunner.Orleans;

public static class OrleansExtensions
{
    public static WebApplicationBuilder AddOrleansTestServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<OrleansTestServices>();
        builder.Services.AddSingleton<Ping>(provider => provider.GetRequiredService<OrleansTestServices>().Ping);
        builder.Services.AddSingleton<Activate>(provider => provider.GetRequiredService<OrleansTestServices>().Activate);

        return builder;
    }
}