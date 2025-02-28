using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Text.Json;

namespace TabKeeper.Common.IndexedDB;

/// <summary>
/// A database definition describing the database schema.
/// </summary>
public sealed record IDBDatabaseDefinition
{
    /// <summary>
    /// The database name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The database version.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// The database store definitions.
    /// </summary>
    public required IDBObjectStoreDefinition[] ObjectStores { get; init; }
}

/// <summary>
/// A store index definition describing the index.
/// </summary>
public sealed record IDBIndexDefinition
{
    /// <summary>
    /// The index name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The index key path.
    /// </summary>
    public required string KeyPath { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public bool MultiEntry { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public bool Unique { get; init; }
}

/// <summary>
/// A store definition describing the store.
/// </summary>
public class IDBObjectStoreDefinition
{
    /// <summary>
    /// 
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string? KeyPath { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public bool AutoIncrement { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public IDBIndexDefinition[] Indexes { get; init; } = [];
}

public abstract class IDBBase
{
    public IJSRuntime JSRuntime { get; }

    public IJSObjectReference JSReference { get; }

    protected IDBBase(IJSRuntime jSRuntime, IJSObjectReference jsReference)
    {
        JSRuntime = jSRuntime;
        JSReference = jsReference;
    }

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return JSReference.DisposeAsync();
    }
}

public class IDBFactory
{
    private static IJSObjectReference? module = null;
    private readonly IJSRuntime jSRuntime;

    public IDBFactory(IJSRuntime jSRuntime)
    {
        this.jSRuntime = jSRuntime;
    }

    private static ValueTask<IJSObjectReference> Import(IJSRuntime jsRuntime)
    {
        return module is null ? Execute() : new(module);

        async ValueTask<IJSObjectReference> Execute() =>
            module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/indexed-wrapper.js");
    }

    public static async ValueTask<IDBDatabase> Open(IJSRuntime jSRuntime, IDBDatabaseDefinition definition)
    {
        var module = await Import(jSRuntime);
        var database = await module.InvokeAsync<IJSObjectReference>("open", definition);
        var info = await database.InvokeAsync<IndexedDatabaseInfo>("getInfo");

        var db = new IDBDatabase(jSRuntime, database, info.Name, info.Version, info.ObjectStoreNames);
        return db;
    }

    public static async ValueTask<(string Name, long Version)[]> Databases(IJSRuntime jSRuntime)
    {
        var module = await Import(jSRuntime);
        var database = await module.InvokeAsync<IndexedDatabasesInfo[]>("databases");
        return [.. database.Select(d => (d.Name, d.Version))];
    }

    public static async ValueTask<bool> DeleteDatabase(IJSRuntime jSRuntime, string name)
    {
        var module = await Import(jSRuntime);
        var result = await module.InvokeAsync<bool>("deleteDatabase", name);
        return result;
    }

    public static DotNetObjectReference<T>? GetObjReferenceWrapper<T>(T? value) where T : class
    {
        return value is { } ? DotNetObjectReference.Create(value) : null;
    }

    private record IndexedDatabaseInfo(string Name, long Version, string[] ObjectStoreNames);

    private record IndexedDatabasesInfo(string Name, long Version);
}

public sealed class IDBDatabase : IDBBase
{
    private bool closed;
    private readonly ConcurrentDictionary<string, IDBObjectStore> stores = [];

    /// <summary>
    /// A string that contains the name of the connected database.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The version of the connected database.
    /// </summary>
    public long Version { get; }

    /// <summary>
    /// A string array containing the names of the object stores currently in the connected database.
    /// </summary>
    public string[] ObjectStoreNames { get; }

    public IDBDatabase(IJSRuntime jSRuntime, IJSObjectReference jsReference, string name, long version, string[] objectStoreNames) : base(jSRuntime, jsReference)
    {
        Name = name;
        Version = version;
        ObjectStoreNames = objectStoreNames;
    }

    public ValueTask<IDBObjectStore> GetObjectStore(string name)
    {
        return stores.TryGetValue(name, out var store) ? new(store) : Execute(this, name);

        static async ValueTask<IDBObjectStore> Execute(IDBDatabase db, string name)
        {
            var objectStore = await db.JSReference.InvokeAsync<IJSObjectReference>("objectStore", name);
            var info = await objectStore.InvokeAsync<IndexedObjectStoreInfo>("getInfo");

            var store = new IDBObjectStore(db.JSRuntime, objectStore, info.Name, info.KeyPath, info.AutoIncrement, info.IndexNames);
            db.stores.TryAdd(name, store);
            return store;
        }
    }

    public ValueTask Close()
    {
        return closed ? new() : Execute(this);

        static ValueTask Execute(IDBDatabase db)
        {
            db.closed = true;
            return db.JSReference.InvokeVoidAsync("close");
        }
    }

    public override ValueTask DisposeAsync()
    {
        return stores.IsEmpty
            ? base.DisposeAsync()
            : new ValueTask(Task.WhenAll(stores.Values.Select(s => s.DisposeAsync().AsTask()).Append(base.DisposeAsync().AsTask())));
    }

    private record IndexedObjectStoreInfo(string Name, string KeyPath, bool AutoIncrement, string[] IndexNames);
}

public sealed class IDBObjectStore : IDBBase
{
    /// <summary>
    /// The store name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The store primary key path.
    /// </summary>
    public string KeyPath { get; }

    /// <summary>
    /// Whether the key is auto incremented by the database.
    /// </summary>
    public bool AutoIncrement { get; }

    /// <summary>
    /// A list with all the index names of the store.
    /// </summary>
    public string[] IndexNames { get; }

    public IDBObjectStore(IJSRuntime jSRuntime, IJSObjectReference jsReference, string name, string keyPath, bool autoIncrement, string[] indexNames) : base(jSRuntime, jsReference)
    {
        Name = name;
        KeyPath = keyPath;
        AutoIncrement = autoIncrement;
        IndexNames = indexNames;
    }

    public ValueTask<int> Count()
    {
        return JSReference.InvokeAsync<int>("count");
    }

    public ValueTask<JsonElement> Get<TKey>(TKey key)
    {
        return JSReference.InvokeAsync<JsonElement>("get", key);
    }

    public ValueTask<T> Get<T, TKey>(TKey key)
    {
        return JSReference.InvokeAsync<T>("get", key);
    }

    //public ValueTask<JsonElement> GetKey()
    //{
    //    return JSReference.InvokeAsync<JsonElement>("get", key);
    //}

    //public ValueTask<TKey> GetKey<TKey>()
    //{
    //    return JSReference.InvokeAsync<TKey>("get", key);
    //}

    public ValueTask<IEnumerable<JsonElement>> GetAll()
    {
        return JSReference.InvokeAsync<IEnumerable<JsonElement>>("getAll");
    }

    public ValueTask<IEnumerable<T>> GetAll<T>()
    {
        return JSReference.InvokeAsync<IEnumerable<T>>("getAll");
    }

    public ValueTask<IEnumerable<JsonElement>> GetAllKeys()
    {
        return JSReference.InvokeAsync<IEnumerable<JsonElement>>("getAllKeys");
    }

    public ValueTask<IEnumerable<TKey>> GetAllKeys<TKey>()
    {
        return JSReference.InvokeAsync<IEnumerable<TKey>>("getAllKeys");
    }

    public ValueTask Add<T>(T obj)
    {
        return JSReference.InvokeVoidAsync("add", obj);
    }

    public ValueTask Put<T>(T obj)
    {
        return JSReference.InvokeVoidAsync("put", obj);
    }

    public ValueTask Delete<TKey>(TKey key)
    {
        return JSReference.InvokeVoidAsync("delete", key);
    }

    public ValueTask Clear()
    {
        return JSReference.InvokeVoidAsync("clear");
    }

    //public ValueTask OpenCursor<T>()
    //{
    //}

    //public ValueTask OpenKeyCursor<T>()
    //{
    //}
}
