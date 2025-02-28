using Core.Reactive;
using TabKeeper.People;

namespace TabKeeper.Tabs;

public sealed class TabPersonViewModel : RxObject
{
    private readonly SourceCache<TabProductViewModel, Uuid> products;
    private readonly TabPerson tab;
    private decimal total;

    public TabPerson Tab => tab;

    public Person Person
    {
        get => tab.Person;
        set {
            Tab.Person = value;
            RaisePropertyChanged(nameof(Person));
        }
    }

    public Uuid PersonId => tab.Person.Id;

    public string Name => Person.Name;

    public decimal Total
    {
        get => total;
        private set => SetAndRaise(ref total, value);
    }

    public TabPersonViewModel() : this(new TabPerson(new()))
    {
    }

    public TabPersonViewModel(TabPerson tab)
    {
        this.tab = tab;
        products = new(p => p.ProductId);

        products
            .Connect()
            .AutoRefresh(x => x.Total)
            .AutoRefresh(x => x.Participants)
            .OnItemAdded(x => tab.ProductIds.Add(x.ProductId))
            .OnItemRemoved(x => tab.ProductIds.Remove(x.ProductId))
            .Transform(x => new ProductProxy(PersonId, x))
            .DisposeMany()
            // Get the latest changes and compute the sum.
            .QueryWhenChanged(query => query.Items
                .Sum(proxy => proxy.Total)
            )
            .Subscribe(total => Total = total)
            .DisposeWith(Disposables);
    }

    public None RegisterProduct(TabProductViewModel product)
    {
        products.AddOrUpdate(product);
        return None.Value;
    }

    public None UnRegisterProduct(TabProductViewModel product)
    {
        products.Remove(product);
        return None.Value;
    }

    public None ToggleProduct(TabProductViewModel product)
    {
        return HasProduct(product) ? UnRegisterProduct(product) : RegisterProduct(product);
    }

    public bool HasProduct(Uuid productId)
    {
        return products.Lookup(productId).HasValue;
    }

    public bool HasProduct(TabProductViewModel product)
    {
        return HasProduct(product.ProductId);
    }

    public override void Dispose()
    {
        products.Clear();
        products.Dispose();
        base.Dispose();
    }
}

file sealed class ProductProxy : IDisposable
{
    private readonly IDisposable sub;
    private readonly TabProductViewModel product;

    public decimal Total { get; private set; }

    public ProductProxy(Uuid personId, TabProductViewModel product)
    {
        this.product = product;
        sub = this.product.Participate(personId).Subscribe(total => Total = total);
    }

    public void Dispose()
    {
        sub.Dispose();
    }
}
