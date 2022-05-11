using Orleans;
using OrleansSut.Shared;

namespace TestRunner.Orleans;

public class OrleansTestServices
{
    private readonly IClusterClient _client;

    public OrleansTestServices(IClusterClient client) => _client = client;

    public async Task Ping(object handle, string name)
    {
        var grain = handle as IPingPongGrain ??
                    throw new ArgumentException($"Handle needs to be of type {nameof(IPingPongGrain)}", nameof(handle));
        
        var pong = await grain.Ping(new PingMessage(name));

        var expectedResponse = "Hello " + name;

        if (pong.Response != expectedResponse)
            throw new Exception($"Received response '{pong.Response}' but expected '{expectedResponse}'");
    }

    public async Task<object> Activate(string id)
    {
        var grain = _client.GetGrain<IPingPongGrain>(id);
        await grain.Ping(new PingMessage(string.Empty));
        return grain;
    }
}