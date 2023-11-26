namespace Rhythia.Server.Hubs;

public class ClientState
{
    public readonly string ConnectionId;
    public readonly int UserId;

    public ClientState(in string connectionId, in int userId)
    {
        ConnectionId = connectionId;
        UserId = userId;
    }
}