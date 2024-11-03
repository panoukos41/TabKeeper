using Blazored.LocalStorage;

namespace TabKeeper.Common;

public sealed class LocalStorageConfigurationProvider : ConfigurationProvider, IConfigurationSource
{
    private readonly ISyncLocalStorageService localStorage;

    public LocalStorageConfigurationProvider(ISyncLocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public override bool TryGet(string key, out string? value)
    {
        if (base.TryGet(key, out value))
            return true;

        value = localStorage.GetItemAsString(key);
        if (value is null)
            return false;

        base.Set(key, value);
        return true;
    }

    public override void Set(string key, string? value)
    {
        localStorage.SetItemAsString(key, value ?? string.Empty);
        base.Set(key, value);
    }

    public override void Load()
    {
        foreach (var key in localStorage.Keys())
        {
            base.Set(key, localStorage.GetItemAsString(key));
        }
        OnReload();
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return this;
    }
}
