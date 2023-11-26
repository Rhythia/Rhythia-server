namespace Rhythia.Server.Entities;

// For reference https://github.com/ppy/osu-server-spectator/blob/master/osu.Server.Spectator/Entities/IEntityStore.cs
public interface IEntityStore
{
    int RemainingUsages { get; }
    string EntityName { get; }
    void StopAcceptingEntities(); // We don't use this one yet
}