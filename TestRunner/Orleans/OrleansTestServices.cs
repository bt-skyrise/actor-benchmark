using Orleans;
using OrleansSut.Shared;

namespace TestRunner.Orleans;

public class OrleansTestServices
{
    private readonly IClusterClient _client;

    public OrleansTestServices(IClusterClient client) => _client = client;

    public async Task Ping(string id, string name)
    {
        var grain = _client.GetGrain<IPingPongGrain>(id);
        var pong = await grain.Ping(new PingMessage(name));

        var expectedResponse = "Hello " + name;

        if (pong.Response != expectedResponse)
            throw new Exception($"Received response '{pong.Response}' but expected '{expectedResponse}'");
    }

    public async Task Activate(string id)
    {
        var grain = _client.GetGrain<IPingPongGrain>(id);
        await grain.Ping(new PingMessage(string.Empty));
    }
}