namespace DemoInterop.Shared.Navigation;

/// <summary>
/// One interop mechanism as the site sees it: its display number matching the article section
/// (e.g. "1.1", "1.2", "2"), the URL slug of its dedicated page, the label used in the navigation,
/// which section it belongs to, and whether WebAssembly is the only host that can run it.
/// The page itself repeats the slug in its <c>@page</c> directive, because a route template has
/// to be a literal — keep the two in step when adding a mechanism.
/// </summary>
/// <remarks>
/// Public because the WebAssembly host's sidebar menu is outside this library and renders its
/// items from <see cref="Mechanisms.All"/>.
/// </remarks>
public sealed record MechanismEntry(
    string DisplayNumber,
    string Slug,
    string Label,
    int Section,
    bool WasmOnly);
