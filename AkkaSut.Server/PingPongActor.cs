using Akka.Actor;
using AkkaSut.Shared;

namespace AkkaSut.Server;

public class PingPongActor : ReceiveActor
{
    public PingPongActor()
    {
        ReceiveAsync<PingMessage>(ping =>
        {
            Sender.Tell(new PongMessage("Hello " + ping.Name));
            return Task.CompletedTask;
        });
    }
}