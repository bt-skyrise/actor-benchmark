using Dapr.Actors.Runtime;
using DaprSut.Shared;

namespace DaprSut.Server;

public class PingPongActor : Actor, IPingPongActor
{
    public Task<PongMessage> Ping(PingMessage ping) =>
        Task.FromResult(new PongMessage {Response = "Hello " + ping.Name});

    public PingPongActor(ActorHost host) : base(host)
    {
    }
}