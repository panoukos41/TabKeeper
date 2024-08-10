using Core.Reactive;
using DynamicData.Binding;

namespace TabKeeper.Tabs;

public sealed class TabProductViewModel : RxObject
{
    private TabProduct product;
    private int participants = 0;
    private readonly HashSet<Uuid> participantIds = [];

    public TabProduct Product
    {
        get => product;
        set => SetAndRaise(ref product, value);
    }

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

    public IEnumerable<Uuid> ParticipantIds => participantIds;

    public TabProductViewModel() : this(new() { Id = Uuid.NewUuid() })
    {
    }

    public TabProductViewModel(TabProduct product)
    {
        this.product = product;
    }

    public IObservable<decimal> Participate(Uuid participantId) =>
        Observable.Defer(() =>
        {
            if (participantIds.Add(participantId))
            {
                RaisePropertyChanged(nameof(ParticipantIds));
                Interlocked.Increment(ref participants);
                RaisePropertyChanged(nameof(Participants));
            }
            return this
                .WhenAnyPropertyChanged(nameof(Total), nameof(Participants))
                .Prepend(this)
                .Select(static vm => vm!.Participants > 0 ? vm!.Total / vm!.Participants : 0)
                .Finally(() =>
                {
                    if (participantIds.Remove(participantId))
                    {
                        RaisePropertyChanged(nameof(ParticipantIds));
                        Interlocked.Decrement(ref participants);
                        RaisePropertyChanged(nameof(Participants));
                    }
                });
        });

    public void ProductUpdated()
    {
        RaisePropertiesChanged(nameof(Name), nameof(Price), nameof(Quantity), nameof(Total));
    }
}
