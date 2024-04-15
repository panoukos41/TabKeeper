using Core.Reactive;
using DynamicData.Binding;
using System.Reactive.Subjects;

namespace TabKeeper.Tabs;

public sealed class ProductViewModel : RxObject
{
    private readonly BehaviorSubject<decimal> totalSubject;
    private Product product;
    private int divisor = 0;

    public Uuid ProductId => product.Id;

    public string Name
    {
        get => product.Name;
        set => SetValidateAndRaise(ref product, product with { Name = value });
    }

    public decimal Price
    {
        get => product.Price;
        set => SetValidateAndRaise(ref product, product with { Price = value }, nameof(Price), nameof(Total));
    }

    public decimal Quantity
    {
        get => product.Quantity;
        set => SetValidateAndRaise(ref product, product with { Quantity = value }, nameof(Quantity), nameof(Total));
    }

    public decimal Total => product.Total;

    public int Divisor => divisor;

    public IObservable<decimal> WhenTotalChanged =>
        Observable.Defer(() =>
        {
            Interlocked.Increment(ref divisor);
            RaisePropertyChanged(nameof(Divisor));
            return totalSubject.Finally(() =>
            {
                Interlocked.Decrement(ref divisor);
                RaisePropertyChanged(nameof(Divisor));
            });
        });

    public ProductViewModel() : this(new() { Id = Uuid.NewUuid() })
    {
    }

    public ProductViewModel(Product product)
    {
        this.product = product;
        totalSubject = new(product.Total);
        totalSubject.DisposeWith(Disposables);
        this.WhenValueChanged(x => x.Total).Subscribe(totalSubject);
    }

    public override void Dispose()
    {
        if (!totalSubject.IsDisposed)
            totalSubject.OnCompleted();

        base.Dispose();
    }
}
