namespace Microsoft.AspNetCore.Components;

public static class NavigationManagerMixins
{
    public static void GoBack(this NavigationManager nav, bool forceLoad = false, bool replace = false)
    {
        nav.NavigateTo(nav.Uri.RemovePathSegment(), forceLoad, replace);
    }

    public static void GoBack(this NavigationManager nav, NavigationOptions options)
    {
        nav.NavigateTo(nav.Uri.RemovePathSegment(), options);
    }
}
