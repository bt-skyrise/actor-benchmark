using Orleans;

namespace OrleansSut.Shared;

public interface IPingPongGrain : IGrainWithStringKey
{
    Task<PongMessage> Ping(PingMessage ping);
}

public record PingMessage(string Name);

public record PongMessage(string Response);