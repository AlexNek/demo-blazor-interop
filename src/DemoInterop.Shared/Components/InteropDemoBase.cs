using DemoInterop.Shared.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DemoInterop.Shared.Components;

/// <summary>
/// Plumbing shared by every demo: the JS runtime, the navigation information, the module cache,
/// and releasing the tracked handles when the component goes away. Demos that own extra interop
/// objects override DisposeAsync and chain to base.
/// </summary>
public abstract class InteropDemoBase : ComponentBase, IAsyncDisposable
{
    private DemoModules? _modules;

    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// The app's own base URI, which is the only correct thing to hang an asset path off when
    /// something insists on an absolute URL. See <see cref="DemoAssets.JsAbsolute"/>.
    /// </summary>
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    protected DemoModules Modules => _modules ??= new(JS);

    public virtual ValueTask DisposeAsync() => _modules?.DisposeAsync() ?? default;
}
