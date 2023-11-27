using MessagePack;
using Rhythia.Game.Replays;

namespace Rhythia.Game.Online.Spectator;

[MessagePackObject]
public class StreamData
{
    [Key(0)] public StreamSyncData SyncData { get; set; }
    [Key(1)] public ReplayFrame[] Frames { get; set; } // This class is literally a wrapper for the byte array workaround
    [Key(2)] public byte[] Score { get; set; } // Use byte arrays as a workaround for re-implementation of formats
}