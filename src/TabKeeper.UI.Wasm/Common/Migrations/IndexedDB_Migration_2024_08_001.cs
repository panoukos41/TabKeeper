using Blazored.LocalStorage;
using TabKeeper.People;
using TabKeeper.Tabs;

namespace TabKeeper.Common.Migrations;

public class IndexedDB_Migration_2024_08_001
{
    private const string EntriesKey = "tab-entries";
    private readonly ISyncLocalStorageService localStorage;
    private readonly IndexedDb indexedDb;

    public IndexedDB_Migration_2024_08_001(ISyncLocalStorageService localStorage, IndexedDb indexedDb)
    {
        this.localStorage = localStorage;
        this.indexedDb = indexedDb;
    }

    public async Task Migrate()
    {
        if (localStorage.GetItem<TabEntry[]>(EntriesKey) is not { } tabs)
            return;

        if (await indexedDb.Tabs.CountAsync() is not 0)
            return;

        foreach (var tab in tabs)
        {
            await MigrateTab(tab);
            localStorage.RemoveItem(tab.Id);
        }
        localStorage.RemoveItem(EntriesKey);
    }

    private Task MigrateTab(TabEntry entry)
    {
        if (localStorage.GetItem<TabOld>(entry.Id) is not { } tab)
            return Task.CompletedTask;

        var newTab = new Tab(tab.Id)
        {
            Name = tab.Name,
            Place = string.IsNullOrEmpty(tab.Place) ? null : tab.Place,
            Date = tab.Date,
            Products = [.. tab.Products],
            People = [.. tab.People.Select(x => new TabPerson(x.Person) { ProductIds = x.ProductIds })],
        };
        return indexedDb.Tabs.AddAsync<Tab, Uuid>(newTab);
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
