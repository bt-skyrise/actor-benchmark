using Orleans;
using OrleansSut.Shared;

namespace OrleansSut.Server;

public class PingPongGrain : Grain, IPingPongGrain
{
    public Task<PongMessage> Ping(PingMessage ping) => 
        Task.FromResult(new PongMessage("Hello " + ping.Name));
}