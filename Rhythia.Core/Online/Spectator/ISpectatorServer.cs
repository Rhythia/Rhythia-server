namespace Rhythia.Core.Online.Spectator;

public interface ISpectatorServer
{
    Task StartStreaming(StreamInfo streamInfo);
    Task StopStreaming();
    Task SendStreamData(StreamData streamData);
    Task StartWatching(int userId);
    Task StopWatching(int userId);
}