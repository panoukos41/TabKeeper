using Core.Reactive;
using Ignis.Components;
using Microsoft.AspNetCore.Components;

namespace TabKeeper.Components.Abstractions;

public abstract class RxViewBase<TRxObject> : IgnisComponentBase, IDisposable
    where TRxObject : RxObject
{
    private IDisposable? changedSub;

    [Parameter, EditorRequired]
    public TRxObject ViewModel { get; set; } = default!;

    protected override bool ShouldRender => ViewModel is not null;

    protected override void OnUpdate()
    {
        changedSub?.Dispose();
        changedSub = ViewModel?.WhenPropertyChanged.Subscribe(_ => Update());
    }

    public void Dispose()
    {
        changedSub?.Dispose();
        changedSub = null;
        GC.SuppressFinalize(this);
    }
}
