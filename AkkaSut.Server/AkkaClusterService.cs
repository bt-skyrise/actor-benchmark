using Akka.Actor;
using Akka.Cluster.Sharding;
using AkkaSut.Shared;

namespace AkkaSut.Server;

public class AkkaClusterService : IHostedService
{
    private readonly ActorSystem _actorSystem;
    private readonly ClusterSharding _sharding;
    private readonly IConfiguration _config;

    public AkkaClusterService(ActorSystem actorSystem, ClusterSharding sharding, IConfiguration config)
    {
        _actorSystem = actorSystem;
        _sharding = sharding;
        _config = config;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var pingPongProps = Props.Create(() => new PingPongActor());


        await _sharding.StartAsync(
            typeName: Consts.PingPongShardTypeName,
            entityProps: pingPongProps,
            settings: ClusterShardingSettings.Create(_actorSystem).WithRole(_config["Akka:Role"]),
            messageExtractor: new MessageExtractor(30)
        );

    }

    public Task StopAsync(CancellationToken cancellationToken) => 
        CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
}