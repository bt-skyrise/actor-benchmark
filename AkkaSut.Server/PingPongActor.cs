using Akka.Actor;
using AkkaSut.Shared;

namespace AkkaSut.Server;

public class PingPongActor : ReceiveActor
{
    public PingPongActor()
    {
        Receive<PingMessage>(ping =>
        {
            Sender.Tell(new PongMessage("Hello " + ping.Name));
        });
    }
}
