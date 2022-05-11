using TestRunner.Tests;

namespace TestRunner.Dapr;


    public static class DaprExtensions
    {
        public static void AddDaprTestServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<Ping>(DaprTestServices.Ping);
            builder.Services.AddSingleton<Activate>(DaprTestServices.Activate);
        }
    }
