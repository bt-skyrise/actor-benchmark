using Proto.Cluster;
using ProtoActorSut.Contracts;

namespace TestRunner.ProtoActor;

public class ProtoActorTestServices
{
    private readonly Cluster _cluster;

    public ProtoActorTestServices(Cluster cluster) => _cluster = cluster;
    
    public async Task Ping(string id, string name)
    {
        var actor = _cluster.GetPingPongActor(id);
        var pong = await actor.Ping(new PingMessage { Name = name}, CancellationToken.None);
        
        var expectedResponse = "Hello " + name;

        if (pong?.Response != expectedResponse)
            throw new Exception($"Received response '{pong?.Response}' but expected '{expectedResponse}'");
    }

    public async Task Activate(string id)
    {
        var actor = _cluster.GetPingPongActor(id);
        await actor.Ping(new PingMessage { Name = string.Empty}, CancellationToken.None);
    }
}