using Core.Reactive;
using DynamicData.Binding;

namespace TabKeeper.Tabs;

public sealed class TabProductViewModel : RxObject
{
    private TabProduct product;
    private int participants = 0;

    public TabProduct Product => product;

    public Uuid ProductId => product.Id;

    public string Name
    {
        get => product.Name;
        set => SetAndRaise(ref product, product with { Name = value });
    }

    public decimal Price
    {
        get => product.Price;
        set => SetAndRaise(ref product, product with { Price = value }, nameof(Price), nameof(Total));
    }

    public decimal Quantity
    {
        get => product.Quantity;
        set => SetAndRaise(ref product, product with { Quantity = value }, nameof(Quantity), nameof(Total));
    }

    public decimal Total => product.Total;

    public int Participants => participants;

    public TabProductViewModel() : this(new() { Id = Uuid.NewUuid() })
    {
    }

    public TabProductViewModel(TabProduct product)
    {
        this.product = product;
    }

    public IObservable<decimal> Participate() =>
        Observable.Defer(() =>
        {
            Interlocked.Increment(ref participants);
            RaisePropertyChanged(nameof(Participants));

            return this
                .WhenValueChanged(x => x.Total)
                .Select(total => Participants > 0 ? total / Participants : 0)
                .Finally(() =>
                {
                    Interlocked.Decrement(ref participants);
                    RaisePropertyChanged(nameof(Participants));
                });
        });

    public void ProductUpdated()
    {
        RaisePropertiesChanged(nameof(Name), nameof(Price), nameof(Quantity), nameof(Total));
    }
}
