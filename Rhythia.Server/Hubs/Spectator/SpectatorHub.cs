using Rhythia.Core.Online.Spectator;
using Rhythia.Server.Entities;

namespace Rhythia.Server.Hubs.Spectator;

public class SpectatorHub : StatefulUserHub<ISpectatorClient, SpectatorClientState>, ISpectatorServer
{
    public SpectatorHub(EntityStore<SpectatorClientState> users) : base(users)
    {}
    public static string GetGroupId(string userId) => $"watch/{userId}";
    public string CurrentContextGroupId => GetGroupId(CurrentContextUserId);
    
    // Broadcast connections to all players for now since we lack any kind of presence
    public override async Task UserConnected()
    {
        await base.UserConnected();
        await Clients.Others.PlayerAdded(Context.GetUserId(), Context.GetUserName());
        foreach (var state in GetAllStates())
        {
            if (state.Key == CurrentContextUserId) continue;
            Clients.Caller.PlayerAdded(state.Value.UserId, state.Value.StreamInfo?.UserName ?? "unknown");
        }
    }
    public override async Task UserDisconnected()
    {
        await base.UserDisconnected();
        await Clients.Others.PlayerRemoved(Context.GetUserId());
    }

    public async Task StartStreaming(StreamInfo streamInfo)
    {
        using var usage = await GetOrCreateLocalUserState();
        var clientState = (usage.Item ??= new SpectatorClientState(Context.ConnectionId, CurrentContextUserId));
        
        if (clientState.StreamInfo != null) throw new Exception(); // Missing "InvalidStateException" TODO: Handle this properly
        
        clientState.StreamInfo = streamInfo;
        streamInfo.UserName = Context.GetUserName();
        if (streamInfo.MapId == null) throw new ArgumentNullException(nameof(streamInfo.MapId));
        if (streamInfo.Mods == null) throw new ArgumentNullException(nameof(streamInfo.Mods));
        if (streamInfo.Settings == null) throw new ArgumentNullException(nameof(streamInfo.Settings));
        // Allow "Score" to be null since it's not a requirement for replays
        
        Console.WriteLine($"{CurrentContextUserId} started streaming");
        await Clients.Others.StreamStarted(CurrentContextUserId, streamInfo); // Broadcast to everyone
    }

    public async Task StopStreaming()
    {
        using var usage = await GetOrCreateLocalUserState();
        usage.Destroy();

        Console.WriteLine($"{CurrentContextUserId} stopped streaming");
        await stopStreaming(CurrentContextUserId);
    }

    public async Task SendStreamData(StreamData streamData)
    {
        using var usage = await GetOrCreateLocalUserState();
        var info = usage.Item?.StreamInfo;
        if (info == null)
            return; // TODO: Err... handle this properly later

        info.Score = streamData.Score;
        info.SyncData = streamData.SyncData;
        
        await Clients.Group(CurrentContextGroupId).StreamDataReceived(CurrentContextUserId, streamData);
    }

    public async Task StartWatching(string userId)
    {
        try
        {
            StreamInfo? streamInfo;
            using (var usage = await GetStateFromUser(userId))
                streamInfo = usage.Item?.StreamInfo;
            if (streamInfo != null)
                await Clients.Caller.StreamStarted(userId, streamInfo);
        } catch {}

        string group = GetGroupId(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }
    public async Task StopWatching(string userId)
    {
        string group = GetGroupId(userId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }

    protected override async Task CleanUpState(SpectatorClientState state)
    {
        if (state.StreamInfo != null)
            await stopStreaming(state.UserId);
        await base.CleanUpState(state);
    }

    private async Task stopStreaming(string userId)
    {
        await Clients.Others.StreamEnded(userId);
        // await Clients.Group(GetGroupId(userId)).StreamEnded(userId);
    }
}