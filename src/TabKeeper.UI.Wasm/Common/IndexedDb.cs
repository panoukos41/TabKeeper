using CloudNimble.BlazorEssentials.IndexedDb;
using Microsoft.JSInterop;

namespace TabKeeper.Common;

public sealed class IndexedDb : IndexedDbDatabase
{
    [Index(Name = "Name", Path = "name")]
    public IndexedDbObjectStore People { get; private set; } = null!;

    [Index(Name = "Name", Path = "name")]
    [Index(Name = "Place", Path = "place")]
    [Index(Name = "PayerId", Path = "payerId")]
    [Index(Name = "Date", Path = "date")]
    [Index(Name = "IsLocked", Path = "isLocked")]
    public IndexedDbObjectStore Tabs { get; private set; } = null!;

    public IndexedDb(IJSRuntime jsRuntime) : base(jsRuntime)
    {
        Name = "TabKeeper";
        Version = 1;

        // Indexes are added to the Store in the constructor, so nothing else is needed here.
        // People
        //_ = new IndexedDbIndex(People, "Name", "name");

        // Tabs
        //_ = new IndexedDbIndex(Tabs, "Name", "name");
        //_ = new IndexedDbIndex(Tabs, "Place", "place");
        //_ = new IndexedDbIndex(Tabs, "PayerId", "payerId");
        //_ = new IndexedDbIndex(Tabs, "Date", "date");
        //_ = new IndexedDbIndex(Tabs, "IsLocked", "isLocked");
    }
}
