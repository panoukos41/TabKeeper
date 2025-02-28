namespace TabKeeper.Layout;

public readonly record struct ShellContext
{
    public string Title { get; init; }

    public string? BackUri { get; init; }

    public ShellContext()
    {
        Title = string.Empty;
    }
}
