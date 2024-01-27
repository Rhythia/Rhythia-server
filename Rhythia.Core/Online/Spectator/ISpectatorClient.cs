namespace Rhythia.Core.Online.Spectator;

public interface ISpectatorClient
{
    Task PlayerAdded(string userId, string userName); // TODO: add actual presence
    Task PlayerRemoved(string userId);
    Task StreamStarted(string userId, StreamInfo streamInfo);
    Task StreamEnded(string userId);
    Task StreamDataReceived(string userId, StreamData streamData);
}