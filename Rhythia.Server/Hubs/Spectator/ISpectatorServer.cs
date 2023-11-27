namespace Rhythia.Server.Hubs.Spectator;

public interface ISpectatorServer
{
    Task StartStreaming();
    Task StopStreaming();
    Task SendStreamData();
    Task StartWatching(int userId);
    Task StopWatching(int userId);
}