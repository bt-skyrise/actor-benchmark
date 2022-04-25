using Akka.Actor;
using Akka.Cluster.Sharding;
using AkkaSut.Shared;

namespace TestRunner.Akka;

public class ClusterProxyHostedService : IHostedService
{
    private readonly ClusterSharding _sharding;
    private readonly ActorSystem _actorSystem;
    private readonly IConfiguration _config;

    public ClusterProxyHostedService(ClusterSharding sharding, ActorSystem actorSystem, IConfiguration config)
    {
        _sharding = sharding;
        _actorSystem = actorSystem;
        _config = config;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _sharding.StartProxyAsync(
            typeName: Consts.PingPongShardTypeName,
            _config["Akka:SutRole"],
            new MessageExtractor(maxNumberOfShards: 30));
    }

    public Task StopAsync(CancellationToken cancellationToken) => 
        CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
}