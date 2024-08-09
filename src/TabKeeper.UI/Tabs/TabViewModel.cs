using Core.Common.Mixins;
using Core.Reactive;
using DynamicData.Binding;

namespace TabKeeper.Tabs;

public sealed class TabViewModel : RxObject
{
    public Tab Tab { get; }

    private readonly SourceCache<TabProductViewModel, Uuid> products = new(p => p.ProductId);
    private readonly SourceCache<TabPersonViewModel, Uuid> people = new(p => p.PersonId);

    public TabViewModel(Tab tab)
    {
        Tab = tab;

        products.AddOrUpdate(tab.Products.Select(x => new TabProductViewModel(x)));
        people.Edit(people =>
        {
            foreach (var personVm in tab.People.Select(person => new TabPersonViewModel(person)))
            {
                people.AddOrUpdate(personVm);
                foreach (var productVm in products.Items.IntersectBy(personVm.Tab.ProductIds, p => p.ProductId))
                {
                    personVm.RegisterProduct(productVm);
                }
            }
        });

        people
            .Connect()
            .AutoRefresh()
            .OnItemAdded(item => tab.People.AddOrUpdate(item.Tab))
            .OnItemRefreshed(item => tab.People.AddOrUpdate(item.Tab))
            //.OnItemUpdated((item, _) => tab.People.AddOrUpdate(item.Tab))
            .OnItemRemoved(item => tab.People.Remove(item.Tab))
            .DisposeMany()
            .BindToObservableList(out var peopleList)
            .Subscribe(_ => RaisePropertyChanged(nameof(People)))
            .DisposeWith(this);
        People = peopleList;

        products
            .Connect()
            .AutoRefresh()
            .OnItemAdded(item => tab.Products.AddOrUpdate(item.Product))
            .OnItemRefreshed(item => tab.Products.AddOrUpdate(item.Product))
            //.OnItemUpdated((item, _) => tab.Products.AddOrUpdate(item.Product))
            .OnItemRemoved(item => tab.Products.Remove(item.Product))
            .DisposeMany()
            .BindToObservableList(out var productsList)
            .Subscribe(_ => RaisePropertyChanged(nameof(Products)))
            .DisposeWith(this);

        Products = productsList;
    }

    public IObservableList<TabProductViewModel> Products { get; }

    public IObservableList<TabPersonViewModel> People { get; }

    public decimal Total => products.Items.Sum(x => x.Total);

    public void AddProduct(TabProductViewModel product)
    {
        products.AddOrUpdate(product);
    }

    public void RemoveProduct(TabProductViewModel product)
    {
        products.Remove(product);
    }

    public void AddPerson(TabPersonViewModel person)
    {
        people.AddOrUpdate(person);
    }

    public void RemovePerson(TabPersonViewModel person)
    {
        people.Remove(person);
    }
}