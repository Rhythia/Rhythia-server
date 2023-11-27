namespace Rhythia.Game.Online.Spectator;

public interface ISpectatorServer
{
    Task StartStreaming();
    Task StopStreaming();
    Task SendStreamData();
    Task StartWatching(int userId);
    Task StopWatching(int userId);
}