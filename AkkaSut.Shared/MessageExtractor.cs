using Akka.Cluster.Sharding;

namespace AkkaSut.Shared;

public class MessageExtractor : HashCodeMessageExtractor
{
    public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards) { }

    public override string? EntityId(object message) =>
        message switch
        {
            PingMessage ping => ping.ActorId,
            _ => null
        };
}