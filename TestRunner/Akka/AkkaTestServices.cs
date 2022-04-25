using Akka.Actor;
using Akka.Cluster.Sharding;
using AkkaSut.Shared;

namespace TestRunner.Akka;

public class AkkaTestServices
{
    private readonly IActorRef _shard;

    public AkkaTestServices(ClusterSharding clusterSharding)
    {
        _shard = clusterSharding.ShardRegion(Consts.PingPongShardTypeName);
    }

    public async Task Ping(string id, string name)
    {
        var pong = await _shard.Ask<PongMessage>(new PingMessage(id, name));
        
        var expectedResponse = "Hello " + name;
        
        if (pong.Response != expectedResponse)
            throw new Exception($"Received response '{pong.Response}' but expected '{expectedResponse}'");
    }

    public Task Activate(string id) => _shard.Ask<PongMessage>(new PingMessage(id, ""));
}