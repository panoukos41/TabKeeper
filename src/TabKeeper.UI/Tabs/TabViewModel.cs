using Core.Reactive;

namespace TabKeeper.Tabs;

public sealed class TabViewModel : RxObject
{
    private readonly Tab tab;

    public TabViewModel(Tab tab)
    {
        this.tab = tab;
    }
}