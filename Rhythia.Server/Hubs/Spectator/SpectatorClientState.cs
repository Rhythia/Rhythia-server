using Rhythia.Core.Online.Spectator;

namespace Rhythia.Server.Hubs.Spectator;

public class SpectatorClientState : ClientState
{
    public SpectatorClientState(in string connectionId, in string userId) : base(connectionId, userId)
    {}

    public StreamInfo? StreamInfo;
}