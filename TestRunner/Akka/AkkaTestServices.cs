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

    public async Task Ping(object handle, string name)
    {
        var id = handle as string ??
                    throw new ArgumentException("Handle needs to be of type string", nameof(handle));
        
        var pong = await _shard.Ask<PongMessage>(new PingMessage(id, name));
        
        var expectedResponse = "Hello " + name;
        
        if (pong.Response != expectedResponse)
            throw new Exception($"Received response '{pong.Response}' but expected '{expectedResponse}'");
    }

    public async Task<object> Activate(string id)
    {
        await _shard.Ask<PongMessage>(new PingMessage(id, ""));
        return id;
    }
}