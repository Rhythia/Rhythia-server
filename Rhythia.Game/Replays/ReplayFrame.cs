using MessagePack;

namespace Rhythia.Game.Replays;

[MessagePackObject]
public class ReplayFrame
{
    [Key(0)] public int Opcode { get; set; }
    [Key(1)] public double Time { get; set; }
    [Key(2)] public byte[]? Data { get; set; } // Byte array workaround once again
}