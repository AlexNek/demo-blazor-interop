namespace DemoInterop.Shared.Navigation;

/// <summary>
/// One interop mechanism as the site sees it: its number, the URL slug of its dedicated page,
/// the label used in the navigation, and whether WebAssembly is the only host that can run it.
/// The page itself repeats the slug in its <c>@page</c> directive, because a route template has
/// to be a literal — keep the two in step when adding a mechanism.
/// </summary>
/// <remarks>
/// Public because the WebAssembly host's sidebar menu is outside this library and renders its
/// items from <see cref="Mechanisms.All"/>.
/// </remarks>
public sealed record MechanismEntry(int Number, string Slug, string Label, bool WasmOnly);
