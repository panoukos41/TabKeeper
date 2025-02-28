using Blazored.LocalStorage;
using Microsoft.JSInterop;
using TabKeeper.Common.IndexedDB;
using TabKeeper.People;
using TabKeeper.Tabs;

namespace TabKeeper.Common.Migrations;

public sealed class IndexedDB_Migration_2024_08_001
{
    private const string EntriesKey = "tab-entries";
    private readonly ISyncLocalStorageService localStorage;
    private readonly StorageDb storage;

    public IndexedDB_Migration_2024_08_001(ISyncLocalStorageService localStorage, StorageDb storage)
    {
        this.localStorage = localStorage;
        this.storage = storage;
    }

    public async Task Migrate()
    {
        if (localStorage.GetItem<TabEntry[]>(EntriesKey) is not { } tabs)
            return;

        if (await storage.Tabs.Count() is not 0)
            return;

        foreach (var tab in tabs)
        {
            await MigrateTab(tab);
            localStorage.RemoveItem(tab.Id);
        }
        localStorage.RemoveItem(EntriesKey);
    }

    private ValueTask MigrateTab(TabEntry entry)
    {
        if (localStorage.GetItem<TabOld>(entry.Id) is not { } tab)
            return new();

        var newTab = new Tab(tab.Id)
        {
            Name = tab.Name,
            Place = string.IsNullOrEmpty(tab.Place) ? null : tab.Place,
            Date = tab.Date,
            Products = [.. tab.Products],
            People = [.. tab.People.Select(x => new TabPerson(x.Person) { ProductIds = x.ProductIds })],
        };
        return storage.Tabs.Add(newTab);
    }

    private sealed record TabEntry(Uuid Id, string Name);

    private sealed record TabOld : IEntity
    {
        public required Uuid Id { get; init; }

        public string Name { get; set; } = string.Empty;

        public string Place { get; set; } = string.Empty;

        public DateOnly? Date { get; set; }

        public HashSet<TabProduct> Products { get; set; } = [];

        public HashSet<PersonTabOld> People { get; set; } = [];
    }

    private sealed record PersonTabOld
    {
        public Person Person { get; }

        public HashSet<Uuid> ProductIds { get; init; } = [];

        public PersonTabOld(Person person)
        {
            Person = person;
        }

        public override int GetHashCode()
        {
            return Person.Id.GetHashCode();
        }

        public bool Equals(PersonTabOld? other)
        {
            return Person.Id.Equals(other?.Person.Id);
        }
    }
}
