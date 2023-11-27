using MessagePack;

namespace Rhythia.Game.Online.Spectator;

[MessagePackObject]
public class StreamData
{
    [Key(0)] public StreamSyncData SyncData { get; set; }
    [Key(1)] public Array Frames { get; set; } // TODO: Assign replay frame type to this array
}