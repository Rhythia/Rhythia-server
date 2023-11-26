using Rhythia.Server.Entities;

namespace Rhythia.Server.Hubs.Spectator;

public class SpectatorHub : StatefulUserHub<ISpectatorClient, SpectatorClientState>
{
    protected SpectatorHub(EntityStore<SpectatorClientState> users) : base(users)
    {}
}