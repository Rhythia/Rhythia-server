using MessagePack;

namespace Rhythia.Game.Online.Spectator;

[MessagePackObject]
public class StreamData
{
    [Key(0)] public StreamSyncData SyncData { get; set; }
    [Key(1)] public Array Frames { get; set; } // TODO: Assign replay frame type to this array; we can't use the workaround for this since MessagePack can't accept GodotObjects
    [Key(2)] public byte[] Score { get; set; } // Use byte arrays as a workaround for re-implementation of formats
}