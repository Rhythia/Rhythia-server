namespace Rhythia.Server.Entities;

// For reference https://github.com/ppy/osu-server-spectator/blob/master/osu.Server.Spectator/Entities/EntityStore.cs
public class EntityStore<T> : IEntityStore
    where T : class
{
    private readonly Dictionary<long, Entity> entities = new();
    
    private const int lock_timeout = 5000;

    private bool acceptingNewEntities = true;
    public void StopAcceptingEntities() => acceptingNewEntities = false;

    public int RemainingUsages
    {
        get
        {
            lock (entities) return entities.Count;
        }
    }

    public string EntityName => typeof(T).Name;

    public T? GetEntityUnsafe(long id)
    {
        lock (entities) return !entities.TryGetValue(id, out var entity) ? null : entity.GetItemUnsafe();
    }

    public async Task<Usage> GetForUse(long id, bool createOnMissing = false)
    {
        int retries = 0;
        while (retries++ < 10)
        {
            Entity? entity;
            lock (entities)
            {
                if (!entities.TryGetValue(id, out entity))
                {
                    if (!createOnMissing || !acceptingNewEntities)
                        throw new KeyNotFoundException($"Attempted to get unknown entity {EntityName}:{id}");
                    entities[id] = entity = new Entity(id, this);
                }
            }

            try
            {
                await entity.ObtainLockAsync();
            }
            catch (InvalidOperationException)
            {
                if (createOnMissing) continue;
                throw new KeyNotFoundException($"Attempted to get unknown entity {EntityName}:{id}");
            }

            return new Usage(entity);
        }
        throw new TimeoutException("Could not allocate new entity");
    }

    public async Task Destroy(long id)
    {
        Entity? entity;
        lock (entities)
        {
            if (!entities.TryGetValue(id, out entity)) return;
        }

        try
        {
            await entity.ObtainLockAsync();
            entity.Destroy();
        }
        catch (InvalidOperationException) { }
    }

    public KeyValuePair<long, T?>[] GetAllEntities()
    {
        lock (entities)
            return entities
                .Where(pair => pair.Value.GetItemUnsafe() != null)
                .Select(entity => new KeyValuePair<long, T?>(entity.Key, entity.Value.GetItemUnsafe()))
                .ToArray();
    }

    public void Clear()
    {
        lock (entities)
            entities.Clear();
    }
    private void remove(long id)
    {
        lock (entities)
            entities.Remove(id);
    }

    public class Entity
    {
        private readonly SemaphoreSlim semaphore = new(1);
        
        private T? item;
        private readonly long id;
        private readonly EntityStore<T> store;
        
        internal bool Destroyed { get; private set; }
        private bool isLocked => semaphore.CurrentCount == 0;

        public Entity(long id, EntityStore<T> store)
        {
            this.id = id;
            this.store = store;
        }

        public T? GetItemUnsafe() => item;
        public T? GetItemSafe()
        {
            checkValidForUse();
            return item;
        }
        public void SetItemSafe(T? value)
        {
            checkValidForUse();
            item = value;
        }
        public T? Item
        {
            get => GetItemSafe();
            set => SetItemSafe(value);
        }

        public void Destroy()
        {
            if (Destroyed) return;
            checkValidForUse();
            Destroyed = true;
            store.remove(id);
            semaphore.Release();
            semaphore.Dispose();
        }

        public async Task ObtainLockAsync()
        {
            checkValidForUse(false);
            if (!await semaphore.WaitAsync(lock_timeout))
                throw new TimeoutException($"Couldn't obtain lock for {store.EntityName}:{id} within {lock_timeout}ms");
            checkValidForUse();
        }

        public void ReleaseLock()
        {
            if (!Destroyed)
                semaphore.Release();
        }

        private void checkValidForUse(bool shouldBeLocked = true)
        {
            if (Destroyed) throw new InvalidOperationException("Attempted to use destroyed entity");
            if (shouldBeLocked && !isLocked) throw new InvalidOperationException("Attempted to use entity without lock");
        }
    }

    public class Usage : IDisposable
    {
        private readonly Entity entity;

        public T? Item
        {
            get => entity.Item;
            set => entity.Item = value;
        }

        public Usage(in Entity entity)
        {
            this.entity = entity;
        }
        
        private readonly bool disposed = false;
        ~Usage()
        {
            if (!disposed) Dispose();
        }
        public void Dispose()
        {
            if (disposed) throw new InvalidOperationException("Attempted to dispose object twice");
            if (!entity.Destroyed && entity.Item == null)
                entity.Destroy();
            entity.ReleaseLock();
        }
    }
}