using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace TabKeeper;

[SupportedOSPlatform("browser")]
public partial class Theme
{
    private const string name = "WizardCounter.Theme";
    private static int imported;

    public static Task Import(string? url = null)
    {
        if (Interlocked.Exchange(ref imported, 1) is 1)
            return Task.CompletedTask;

        // JSHost starts searching from inside _framework folder so we have to go one up.
        return JSHost.ImportAsync(name, url ?? "../js/theme.js");
    }

    [JSImport("updateDom", name)]
    public static partial void UpdateDom();

    [JSImport("toggle", name)]
    public static partial void Toggle();

    [JSImport("setDark", name)]
    public static partial void SetDark();

    [JSImport("setLight", name)]
    public static partial void SetLight();

    [JSImport("setAuto", name)]
    public static partial void SetAuto();

    [JSImport("getTheme", name)]
    public static partial string GetTheme();
}
