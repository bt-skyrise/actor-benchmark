using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Logger.Extensions.Logging;

namespace AkkaSut.Shared;

public static class ActorSystemExtensions
{
    public static WebApplicationBuilder AddAkkaClusterSharding(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection("Akka");

        var seedNodes = config.GetSection("SeedNodes").Get<List<string>>();
        var seedNodesAkkaConfig = seedNodes
            .Aggregate("", (acc, node) => $"{acc}\"akka.tcp://{config["ActorSystemName"]}@{node}\",")
            .TrimEnd(',');

        var hocon = @$"
            akka {{
                actor.provider = cluster
                cluster.roles = [{config["Role"]}]

                remote.dot-netty.tcp {{
                    hostname = 0.0.0.0
                    public-hostname = {config["PublicHostname"]}
                    port = {config["Port"]}
                }}

                cluster.seed-nodes = [{seedNodesAkkaConfig}]

                loglevel = INFO
                loggers = [""Akka.Logger.Extensions.Logging.LoggingLogger, Akka.Logger.Extensions.Logging""]

                # shard coordinator uses snapshots, use in mem impl to avoid filesystem permission issues
                persistence.snapshot-store {{
                    plugin = ""akka.persistence.snapshot-store.inmem""
                    inmem {{
                        class = ""Akka.Persistence.Snapshot.MemorySnapshotStore, Akka.Persistence""
                        plugin-dispatcher = ""akka.actor.default-dispatcher""
                    }}
                }}

                actor {{
                    # use fast serialization instead of default newtonsoft json
                    serializers {{
                        hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }}

                    serialization-bindings {{
                        ""System.Object"" = hyperion
                    }}
                }}
            }}
            ";

        builder.Services.AddSingleton(sp =>
        {
            LoggingLogger.LoggerFactory = sp.GetRequiredService<ILoggerFactory>();
            
            var bootstrap = BootstrapSetup.Create()
                .WithConfig(ConfigurationFactory.ParseString(hocon));

            var serviceProviderSetup = DependencyResolverSetup.Create(sp);

            bootstrap.And(serviceProviderSetup);
            
            return ActorSystem.Create(config["ActorSystemName"], bootstrap);
        });

        builder.Services.AddSingleton(sp => ClusterSharding.Get(sp.GetRequiredService<ActorSystem>()));

        return builder;
    }
}