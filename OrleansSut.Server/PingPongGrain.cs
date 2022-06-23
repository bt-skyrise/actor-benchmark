using Orleans;
using OrleansSut.Shared;

namespace OrleansSut.Server;

public class PingPongGrain : Grain, IPingPongGrain
{
    public ValueTask<PongMessage> Ping(PingMessage ping) => 
        ValueTask.FromResult(new PongMessage("Hello " + ping.Name));
}