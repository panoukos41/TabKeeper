using Core.Blazor;
using Microsoft.AspNetCore.Components;

namespace TabKeeper.Components;

public partial class LottiePlayer : CoreComponent
{
    /// <summary>
    /// The animation to play, either Bodymovin JSON data or a URL to a JSON file.
    /// </summary>
    /// <remarks>This parameter is mandatory.</remarks>
    [Parameter, EditorRequired]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Number of times to loop the animation
    /// </summary>
    [Parameter]
    public int? Count { get; set; }

    /// <summary>
    /// Duration (in milliseconds) to pause before playing each cycle in a looped animation.
    /// </summary>
    /// <remarks>Set this parameter to 0 (no pause) or any positive number.</remarks>
    [Parameter]
    public int Intermission { get; set; } = 1;

    /// <summary>
    /// Animation speed.
    /// </summary>
    /// <remarks>Set this parameter to any positive number.</remarks>
    [Parameter]
    public int Speed { get; set; } = 1;

    /// <summary>
    /// Direction of the animation. Set to 1 to play the animation forward or set to -1 to play it backward.
    /// </summary>
    /// <value>1 or -1</value>
    [Parameter]
    public LottieDirection Direction { get; set; } = LottieDirection.Forward;

    /// <summary>
    /// Background color. By default, the background is transparent and will take the color of the parent container.
    /// </summary>
    [Parameter]
    public string? Background { get; set; }

    /// <summary>
    /// When set to <see langword="true" />, loops the animation. The <see cref="Count"/> property defines the number of times to loop the animation.
    /// </summary>
    /// <remarks>Setting the <see cref="Count"/> property to 0 and setting <see cref="Loop"/> to <see langword="true" />, loops the animation indefinitely.</remarks>
    [Parameter]
    public bool Loop { get; set; }

    /// <summary>
    /// When set to <see langword="true" />, displays player controls.
    /// </summary>
    [Parameter]
    public bool Controls { get; set; }

    /// <summary>
    /// Play mode. Setting the mode to <see cref="LottiePlayMode.Bounce" /> plays the animation in an indefinite cycle, forwards and then backwards.
    /// </summary>
    [Parameter]
    public LottiePlayMode Mode { get; set; } = LottiePlayMode.Normal;

    /// <summary>
    /// The renderer to use for the animation
    /// </summary>
    /// <value>'svg', 'html' or 'canvas'</value>
    [Parameter]
    public string Renderer { get; set; } = "svg";

    /// <summary>
    /// Disables verification of the Lottie animation before loading it.
    /// </summary>
    [Parameter]
    public bool DisableCheck { get; set; }

    /// <summary>
    /// When set to <see langword="true"/>, plays animation on mouse over
    /// </summary>
    [Parameter]
    public bool Hover { get; set; }

    /// <summary>
    /// When set to <see langword="true"/>, automatically plays the animation on loading it.
    /// </summary>
    [Parameter]
    public bool AutoPlay { get; set; }

    /// <summary>
    /// Valid value to preserve the aspect ratio. See the <a href="https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/preserveAspectRatio">Mozilla documentation</a> for more information.
    /// </summary>
    [Parameter]
    public string PreserveAspectRatio { get; set; } = string.Empty;
}

public enum LottieDirection
{
    Forward = 1,
    Backward = -1,
}

public enum LottiePlayMode
{
    Normal,
    Bounce
}
