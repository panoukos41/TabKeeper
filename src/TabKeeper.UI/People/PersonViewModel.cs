using Core.Reactive;
using TabKeeper.People;
using TabKeeper.UI.Tabs;

namespace TabKeeper.UI.People;

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

    public PersonViewModel(Person person)
    {
        this.person = person;
        products = new(p => p.ProductId);

        var subscription = products.Connect();

        subscription
            .WhereReasonsAre(ChangeReason.Add, ChangeReason.Remove)
            .ForEachChange(c => c.Current.Divisor += c.Reason is ChangeReason.Add ? 1 : -1)
            .Subscribe();

        subscription
            .AutoRefresh(x => x.Total) // will refresh when an object emits it's total changed.
            .QueryWhenChanged(query => query.Items
                .Select(product => product.Divisor > 0 ? product.Total / product.Divisor : 0)
                .Sum(division => division)
            ) // Get the latest changes and compute the sum.
            .Subscribe(total => SetAndRaise(ref this.total, total, nameof(Total))); // Set our total.
    }

    public void AddProduct(ProductViewModel product)
    {
        products.AddOrUpdate(product);
    }

    public void RemoveProduct(ProductViewModel product)
    {
        products.Remove(product);
    }

    public bool HasProduct(Uuid productId)
    {
        return products.Lookup(productId).HasValue;
    }

    public override void Dispose()
    {
        products.Clear();
        products.Dispose();
        base.Dispose();
    }
}
