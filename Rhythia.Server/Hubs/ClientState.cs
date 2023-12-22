namespace Rhythia.Server.Hubs;

public class ClientState
{
    public readonly string ConnectionId;
    public readonly string UserId;
    // public string Username => GetUsernameFromConnectionId();

    public ClientState(in string connectionId, in string userId)
    {
        ConnectionId = connectionId;
        UserId = userId;
    }
}