using Core.Reactive;
using ObservableCollections;
using R3;

namespace TabKeeper.Tabs;

public sealed class TabViewModel : RxObject
{
    public Tab Tab { get; }

    private readonly ObservableList<TabProductViewModel> products = [];
    private readonly ObservableList<TabPersonViewModel> people = [];

    public TabViewModel(Tab tab)
    {
        Tab = tab;
        products.AddRange(tab.Products.Select(x => new TabProductViewModel(x)));
        people.AddRange(tab.People.Select(person =>
        {
            var personVm = new TabPersonViewModel(person);
            foreach (var productVm in products.IntersectBy(personVm.Tab.ProductIds, p => p.ProductId))
            {
                personVm.RegisterProduct(productVm);
            }
            return personVm;
        }));

        products.ForEach(product => product.WhenPropertyChanged.Subscribe(_ =>
        {
            Tab.Products.AddOrUpdate(product.Product);
            RaisePropertyChanged(nameof(Products));
        }));
        people.ForEach(person => person.WhenPropertyChanged.Subscribe(_ =>
        {
            Tab.People.AddOrUpdate(person.Tab);
            RaisePropertyChanged(nameof(People));
        }));

        people.ObserveAdd().Subscribe(e => tab.People.AddOrUpdate(e.Value.Tab)).DisposeWith(this);
        //people.ObserveChanged().Subscribe(e => tab.People.AddOrUpdate(e.Value.Product));
        people.ObserveRemove().Subscribe(e => tab.People.Remove(e.Value.Tab)).DisposeWith(this);
        people.ObserveChanged().Subscribe(_ => RaisePropertiesChanged(nameof(People))).DisposeWith(this);
        People = people.ToViewList();

        products.ObserveAdd().Subscribe(e => tab.Products.AddOrUpdate(e.Value.Product)).DisposeWith(this);
        //products.ObserveChanged().Subscribe(e => tab.Products.AddOrUpdate(e.Value.Product));
        products.ObserveRemove().Subscribe(e => tab.Products.Remove(e.Value.Product)).DisposeWith(this);
        products.ObserveChanged().Subscribe(_ => RaisePropertiesChanged(nameof(Products))).DisposeWith(this);
        Products = products.ToViewList();
    }

    public ISynchronizedViewList<TabProductViewModel> Products { get; }

    public ISynchronizedViewList<TabPersonViewModel> People { get; }

    public decimal Total => products.Sum(x => x.Total);

    public TabProductViewModel? GetProduct(Uuid productId)
    {
        return products.FirstOrDefault(x => x.ProductId == productId);
    }

    public void AddProduct(TabProductViewModel product)
    {
        products.Add(product);
        product.WhenPropertyChanged.Subscribe(_ =>
        {
            Tab.Products.AddOrUpdate(product.Product);
            RaisePropertyChanged(nameof(Products));
        });
    }

    public void RemoveProduct(TabProductViewModel product)
    {
        foreach (var participant in product.ParticipantIds)
        {
            if (GetPerson(participant) is { } person)
            {
                person.UnRegisterProduct(product);
            }
        }
        product.Dispose();
        products.Remove(product);
    }

    public TabPersonViewModel? GetPerson(Uuid personId)
    {
        return people.FirstOrDefault(x => x.PersonId == personId);
    }

    public void AddPerson(TabPersonViewModel person)
    {
        people.Add(person);
        person.WhenPropertyChanged.Subscribe(_ =>
        {
            Tab.People.AddOrUpdate(person.Tab);
            RaisePropertyChanged(nameof(People));
        });
    }

    public void RemovePerson(TabPersonViewModel person)
    {
        person.Dispose();
        people.Remove(person);
    }
}