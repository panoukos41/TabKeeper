using Core.Reactive;
using DynamicData.Binding;
using TabKeeper.People;

namespace TabKeeper.Tabs;

public sealed class TabViewModel : RxObject
{
    public Tab Tab { get; }

    private readonly SourceCache<PersonViewModel, Uuid> people = new(p => p.PersonId);
    private readonly SourceCache<ProductViewModel, Uuid> products = new(p => p.ProductId);

    public TabViewModel(Tab tab)
    {
        Tab = tab;

        people
            .Connect()
            .AutoRefresh()
            .DisposeMany()
            .BindToObservableList(out var peopleList)
            .Subscribe(_ => RaisePropertyChanged(nameof(People)))
            .DisposeWith(Disposables);

        products
            .Connect()
            .AutoRefresh(x => x.Total)
            .AutoRefresh(x => x.Divisor)
            .DisposeMany()
            .BindToObservableList(out var productsList)
            .Subscribe(_ => RaisePropertyChanged(nameof(Products)))
            .DisposeWith(Disposables);

        People = peopleList;
        Products = productsList;

        products.AddOrUpdate(tab.Products.Select(x => new ProductViewModel(x)));
        people.Edit(people =>
        {
            foreach (var personTab in tab.People)
            {
                var vm = new PersonViewModel(personTab.Person);
                people.AddOrUpdate(vm);
                foreach (var productId in personTab.ProductIds)
                {
                    var l = products.Lookup(productId);
                    if (l.HasValue)
                    {
                        vm.RegisterProduct(l.Value);
                    }
                }
            }
        });

        foreach (var product in tab.Products)
        {
        }
    }

    public IObservableList<ProductViewModel> Products { get; }

    public IObservableList<PersonViewModel> People { get; }

    public void AddProduct(ProductViewModel product)
    {
        products.AddOrUpdate(product);
    }

    public void RemoveProduct(ProductViewModel product)
    {
        products.Remove(product);
    }

    public void AddPerson(PersonViewModel person)
    {
        people.AddOrUpdate(person);
    }

    public void RemovePerson(PersonViewModel person)
    {
        people.Remove(person);
    }
}