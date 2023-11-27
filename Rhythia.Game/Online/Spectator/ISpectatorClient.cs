namespace Rhythia.Game.Online.Spectator;

public interface ISpectatorClient
{
    Task StreamStarted(int userId, StreamInfo streamInfo);
    Task StreamEnded(int userId);
    Task StreamDataReceived(int userId, StreamData streamData);
}