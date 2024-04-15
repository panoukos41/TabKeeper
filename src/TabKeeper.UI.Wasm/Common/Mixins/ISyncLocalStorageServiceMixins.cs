namespace Blazored.LocalStorage;

public static class ISyncLocalStorageServiceMixins
{
    public static bool TrySetItem<T>(this ISyncLocalStorageService storage, string key, T data)
    {
        if (storage.ContainKey(key)) 
            return false;

        storage.SetItem(key, data);
        return true;
    }
}
