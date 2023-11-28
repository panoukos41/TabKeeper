namespace TabKeeper.UI.AppWeb.Components;

public sealed class ModalContext
{
    private readonly Func<None> hide;

    public ModalContext(Func<None> hide)
    {
        this.hide = hide;
    }

    public void Hide() => hide();
}
