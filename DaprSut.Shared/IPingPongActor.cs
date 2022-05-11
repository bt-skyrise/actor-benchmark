using Dapr.Actors;

namespace DaprSut.Shared;

public interface IPingPongActor : IActor
{
    Task<PongMessage> Ping(PingMessage ping);
}

public class PingMessage {
    public string Name { get; set; }
    
};

public class PongMessage
{
    public string Response { get; set; }
};