namespace Rhythia.Game.Online.Spectator;

public interface ISpectatorClient
{
    Task StreamStarted(int userId);
    Task StreamEnded(int userId);
    Task StreamDataReceived(int userId);
}