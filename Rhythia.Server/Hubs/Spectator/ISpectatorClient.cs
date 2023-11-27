namespace Rhythia.Server.Hubs.Spectator;

public interface ISpectatorClient
{
    Task StreamStarted(int userId);
    Task StreamEnded(int userId);
    Task StreamDataReceived(int userId);
}