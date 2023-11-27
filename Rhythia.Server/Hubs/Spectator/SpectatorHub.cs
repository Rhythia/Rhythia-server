using Rhythia.Server.Entities;

namespace Rhythia.Server.Hubs.Spectator;

public class SpectatorHub : StatefulUserHub<ISpectatorClient, SpectatorClientState>, ISpectatorServer
{
    protected SpectatorHub(EntityStore<SpectatorClientState> users) : base(users)
    {}
    public static string GetGroupId(int userId) => $"watch/{userId}";
    public string CurrentContextGroupId => GetGroupId(CurrentContextUserId);

    public async Task StartStreaming()
    {
        await Clients.Group(CurrentContextGroupId).StreamStarted(CurrentContextUserId);
    }

    public async Task StopStreaming()
    {
        await Clients.Group(CurrentContextGroupId).StreamEnded(CurrentContextUserId);
    }

    public async Task SendStreamData()
    {
        await Clients.Group(CurrentContextGroupId).StreamDataReceived(CurrentContextUserId);
    }

    public async Task StartWatching(int userId)
    {
        string group = GetGroupId(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }
    public async Task StopWatching(int userId)
    {
        string group = GetGroupId(userId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }

}