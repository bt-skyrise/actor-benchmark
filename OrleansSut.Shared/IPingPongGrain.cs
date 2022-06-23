using Orleans;

namespace OrleansSut.Shared;

public interface IPingPongGrain : IGrainWithStringKey
{
    ValueTask<PongMessage> Ping(PingMessage ping);
}

public readonly record struct PingMessage(string Name);

public readonly record struct PongMessage(string Response);