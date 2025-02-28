using Microsoft.JSInterop;
using TabKeeper.Common.IndexedDB;
using TabKeeper.People;
using TabKeeper.Tabs;

namespace TabKeeper.Common;

public static class StorageDefinitions
{
    public static readonly IDBDatabaseDefinition TabKeeper_v1 = new()
    {
        Name = "TabKeeper",
        Version = 1,
        ObjectStores =
        [
            new()
            {
                Name = "People",
                KeyPath = "id",
                Indexes = [
                    new() { Name = "Name", KeyPath = "name" },
                ]
            },
            new()
            {
                Name = "Tabs",
                KeyPath = "id",
                Indexes = [
                    new() { Name = "Name", KeyPath = "name" },
                    new() { Name = "Place", KeyPath = "place" },
                    new() { Name = "PayerId", KeyPath = "payerId" },
                    new() { Name = "Date", KeyPath = "date" },
                    new() { Name = "IsLocked", KeyPath = "isLocked" },
                ]
            },
        ]
    };
}

public sealed class StorageDb : IAsyncDisposable
{
    private readonly IJSRuntime jSRuntime;
    private IDBDatabase db = null!;

    public StorageDb(IJSRuntime jSRuntime)
    {
        this.jSRuntime = jSRuntime;
    }

    public async Task Initialize()
    {
        db = await IDBFactory.Open(jSRuntime, StorageDefinitions.TabKeeper_v1);
        Tabs = new(await db.GetObjectStore("Tabs"));
        People = new(await db.GetObjectStore("People"));
    }

    public StorageStore<Tab, Uuid> Tabs { get; private set; } = null!;

    public StorageStore<Person, Uuid> People { get; private set; } = null!;

    public ValueTask DisposeAsync()
    {
        return db?.DisposeAsync() ?? new();
    }
}

public sealed class StorageStore<T, TKey>
{
    private readonly IDBObjectStore store;

    public StorageStore(IDBObjectStore store)
    {
        this.store = store;
    }

    public ValueTask<int> Count()
    {
        return store.Count();
    }

    public ValueTask<T> Get(TKey key)
    {
        return store.Get<T, TKey>(key);
    }

    public ValueTask<IEnumerable<T>> GetAll()
    {
        return store.GetAll<T>();
    }

    public ValueTask<IEnumerable<TKey>> GetAllKeys()
    {
        return store.GetAllKeys<TKey>();
    }

    public ValueTask Add(T obj)
    {
        return store.Add(obj);
    }

    public ValueTask Put(T obj)
    {
        return store.Put(obj);
    }

    public ValueTask Delete(TKey key)
    {
        return store.Delete(key);
    }

    public ValueTask Clear()
    {
        return store.Clear();
    }
}
