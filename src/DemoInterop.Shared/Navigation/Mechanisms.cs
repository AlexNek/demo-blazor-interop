namespace DemoInterop.Shared.Navigation;

/// <summary>
/// The eight mechanisms in the order the demo presents them. The in-page navigation, the prev/next
/// pager and the WebAssembly host's sidebar menu all read this one list, so a new mechanism is
/// registered in a single place.
/// </summary>
/// <remarks>
/// Public because the sidebar menu lives in the host project, outside this library.
/// </remarks>
public static class Mechanisms
{
    /// <summary>
    /// Prefix every dedicated page hangs behind. Deliberately without a leading slash: a link
    /// whose href starts with one resolves against the <em>site</em> root, which throws away the
    /// <c>/demo-blazor-interop/</c> segment on GitHub Pages. Relative hrefs resolve against the
    /// app's <c>&lt;base href&gt;</c> and keep working under a project site. The host's sidebar menu
    /// relies on this too: it builds its hrefs from <see cref="Href(MechanismEntry)"/>.
    /// </summary>
    public const string RoutePrefix = "mechanism/";

    public static IReadOnlyList<MechanismEntry> All { get; } =
    [
        new(1, "1-ijsruntime", "IJSRuntime by name", false),
        new(2, "2-es-modules", "ES module imports", false),
        new(3, "3-in-process", "In-process, synchronous", true),
        new(4, "4-dotnet-object-reference", "DotNetObjectReference", false),
        new(5, "5-jsexport", "[JSExport]", true),
        new(6, "6-jsimport", "[JSImport]", true),
        new(7, "7-streams", "Stream references", false),
        new(8, "8-dotnet-10-additions", ".NET 10 additions", false),
    ];

    /// <summary>The href for a mechanism page, relative to the app's base URI.</summary>
    public static string Href(MechanismEntry entry) => RoutePrefix + entry.Slug;

    /// <summary>The href for a mechanism page by number, for the cross-links inside the prose.</summary>
    public static string Href(int number) => RoutePrefix + All[number - 1].Slug;

    /// <summary>
    /// The availability line, for a page header that owns its own heading. Read from the same
    /// flag the navigation prints its "WebAssembly only" tag from, so the two cannot drift apart.
    /// </summary>
    public static string Availability(int number) => Availability(All[number - 1].WasmOnly);

    /// <summary>The availability line for a shell that was told the flag directly.</summary>
    public static string Availability(bool wasmOnly) =>
        wasmOnly ? "Blazor WebAssembly only" : "Blazor Server and Blazor WebAssembly";

    /// <summary>The mechanism before this one, or null on the first page.</summary>
    public static MechanismEntry? Previous(int number) => number > 1 ? All[number - 2] : null;

    /// <summary>The mechanism after this one, or null on the last page.</summary>
    public static MechanismEntry? Next(int number) => number < All.Count ? All[number] : null;
}
