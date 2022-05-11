using Dapr.Actors;
using Dapr.Actors.Client;
using DaprSut.Shared;

namespace TestRunner.Dapr;

public class DaprTestServices
{
    public static async Task Ping(object handle, string name)
    {
        var proxy = handle as IPingPongActor ??
                    throw new ArgumentException($"Handle needs to be of type {nameof(IPingPongActor)}", nameof(handle));

        var pong = await proxy.Ping(new PingMessage {Name = name});

        var expectedResponse = "Hello " + name;

        if (pong.Response != expectedResponse)
            throw new Exception($"Received response '{pong.Response}' but expected '{expectedResponse}'");
    }

    public static async Task<object> Activate(string id)
    {
        var proxy = ActorProxy.Create<IPingPongActor>(new ActorId(id), "PingPongActor");
        await proxy.Ping(new PingMessage());
        return proxy;
    }
}