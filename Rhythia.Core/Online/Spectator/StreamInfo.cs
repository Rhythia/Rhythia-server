using MessagePack;

namespace Rhythia.Core.Online.Spectator;

[MessagePackObject]
public class StreamInfo
{
    [Key(0)] public string? MapId { get; set; } // This is NOT the id of the mapset
    [Key(1)] public byte[]? Mods { get; set; } // Use byte arrays as a workaround for re-implementation of formats
    [Key(2)] public byte[]? Settings { get; set; }
    [Key(3)] public byte[]? Score { get; set; }
    [Key(4)] public StreamSyncData SyncData { get; set; }
}