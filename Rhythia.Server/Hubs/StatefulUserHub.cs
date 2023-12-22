using Microsoft.AspNetCore.SignalR;
using Rhythia.Server.Entities;

namespace Rhythia.Server.Hubs;

// For reference https://github.com/ppy/osu-server-spectator/blob/master/osu.Server.Spectator/Hubs/StatefulUserHub.cs
public class StatefulUserHub<TClient, TUserState> : Hub<TClient>
    where TClient : class
    where TUserState : ClientState
{
    protected string CurrentContextUserId
    {
        get => Context.GetUserId();
    }
    
    protected readonly EntityStore<TUserState> UserStates;

    protected StatefulUserHub(EntityStore<TUserState> userStates)
    {
        UserStates = userStates;
    }

    protected KeyValuePair<string, TUserState>[] GetAllStates() => UserStates.GetAllEntities();

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        try
        {
            await cleanUpState(false);
        }
        catch
        {
            Context.Abort();
            throw;
        }
    }

    public sealed override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        await cleanUpState(true);
    }

    private async Task cleanUpState(bool isDisconnect)
    {
        EntityStore<TUserState>.Usage? usage;
        try
        {
            usage = await UserStates.GetForUse(CurrentContextUserId);
        }
        catch (KeyNotFoundException)
        {
            return;
        }

        try
        {
            if (usage.Item != null)
            {
                bool isOurState = usage.Item.ConnectionId == Context.ConnectionId;
                if (isDisconnect && !isOurState) return;
                try
                {
                    await CleanUpState(usage.Item);
                }
                finally
                {
                    usage.Destroy();
                }
            }
        }
        finally
        {
            usage.Dispose();
        }
    }

    protected virtual Task CleanUpState(TUserState state) => Task.CompletedTask;

    protected async Task<EntityStore<TUserState>.Usage> GetOrCreateLocalUserState()
    {
        var usage = await UserStates.GetForUse(CurrentContextUserId, true);
        if (usage.Item != null && usage.Item.ConnectionId != Context.ConnectionId)
        {
            usage.Dispose();
            throw new Exception(); // Missing "InvalidStateException"
        }

        return usage;
    }

    protected Task<EntityStore<TUserState>.Usage> GetStateFromUser(string userId) => UserStates.GetForUse(userId);
}