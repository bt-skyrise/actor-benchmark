namespace AkkaSut.Shared;

public record PingMessage(string ActorId, string Name);

public record PongMessage(string Response);
