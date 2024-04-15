using Core.Reactive;
using TabKeeper.Tabs;

namespace TabKeeper.People;

public sealed class PersonViewModel : RxObject
{
    private readonly SourceCache<ProductViewModel, Uuid> products;
    private Person person;
    private decimal total;

    public Uuid PersonId => person.Id;

    public string Name
    {
        get => person.Name;
        set => SetValidateAndRaise(ref person, person with { Name = value });
    }

    public decimal Total => total;

    public IEnumerable<Uuid> ProductIds => products.Keys;

    public PersonViewModel() : this(new Person { Id = Uuid.NewUuid() })
    {
    }

    public PersonViewModel(Person person)
    {
        this.person = person;
        products = new(p => p.ProductId);

        products
            .Connect()
            .AutoRefresh(x => x.Total) // will refresh when product total has changed.
            .AutoRefresh(x => x.Divisor) // will refresh when product total has changed.
            .Transform(x => new ProductProxy(x))
            .DisposeMany()
            .QueryWhenChanged(query => query.Items
                .Sum(proxy => proxy.Total)
            )
            // Get the latest changes and compute the sum.

            //.QueryWhenChanged(query => query.Items
            //    .Select(product => product.TotalSubs > 0 ? product.Total / product.TotalSubs : 0)
            //    .Sum(division => division)
            //) // Get the latest changes and compute the sum.
            .Subscribe(total => SetAndRaise(ref this.total, total, nameof(Total))) // Set our total.
            .DisposeWith(Disposables);
    }

    public None RegisterProduct(ProductViewModel product)
    {
        products.AddOrUpdate(product);
        return None.Value;
    }

    public None UnRegisterProduct(ProductViewModel product)
    {
        products.Remove(product);
        return None.Value;
    }

    public None ToggleProduct(ProductViewModel product)
    {
        return HasProduct(product) ? UnRegisterProduct(product) : RegisterProduct(product);
    }

    public bool HasProduct(Uuid productId)
    {
        return products.Lookup(productId).HasValue;
    }

    public bool HasProduct(ProductViewModel product)
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
    private readonly ProductViewModel product;

    public decimal Total => product.Divisor > 0 ? product.Total / product.Divisor : 0;

    public ProductProxy(ProductViewModel product)
    {
        this.product = product;
        sub = this.product.WhenTotalChanged.Subscribe();
    }

    public void Dispose()
    {
        sub.Dispose();
    }
}
