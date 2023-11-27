using MessagePack;

namespace Rhythia.Game.Online.Spectator;

[MessagePackObject]
public class StreamInfo
{
    [Key(0)] public string? MapId { get; set; } // This is NOT the id of the mapset
    [Key(1)] public StreamSyncData SyncData { get; set; }
}