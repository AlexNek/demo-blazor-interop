namespace DemoInterop.Shared.Navigation;

/// <summary>
/// The mechanisms in the order the article presents them. The in-page navigation, the prev/next
/// pager and the WebAssembly host's sidebar menu all read this one list, so a new mechanism is
/// registered in a single place.
/// </summary>
/// <remarks>
/// <para>
/// The catalog follows the article's two-section structure: section 1 covers IJSRuntime and its
/// additions (subsections 1.1–1.6), section 2 covers source generators. Disposal (article 1.7)
/// has no demo page and is not in the catalog.
/// </para>
/// <para>
/// Public because the sidebar menu lives in the host project, outside this library.
/// </para>
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

    /// <summary>
    /// The mechanisms in article order. The catalog position (not the display number) determines
    /// the previous/next pager sequence.
    /// </summary>
    public static IReadOnlyList<MechanismEntry> All { get; } =
    [
        // Section 1 — IJSRuntime: the core mechanism
        new("1.1", "1-ijsruntime", "Window globals", 1, false),
        new("1.2", "2-es-modules", "ES modules", 1, false),
        new("1.3", "3-in-process", "Synchronous calls", 1, true),
        new("1.4", "7-streams", "Stream references", 1, false),
        new("1.5", "8-dotnet-10-additions", ".NET 10 additions", 1, false),
        new("1.6", "4-dotnet-object-reference", "Callbacks", 1, false),
        // Section 2 — Source generators
        new("2", "5-jsexport", "[JSExport] and [JSImport]", 2, true),
        new("2", "6-jsimport", "[JSExport] and [JSImport]", 2, true),
    ];

    /// <summary>The section 1 entries (IJSRuntime and additions).</summary>
    public static IEnumerable<MechanismEntry> Section1 => All.Where(e => e.Section == 1);

    /// <summary>The section 2 entries (source generators).</summary>
    public static IEnumerable<MechanismEntry> Section2 => All.Where(e => e.Section == 2);

    /// <summary>The href for a mechanism page, relative to the app's base URI.</summary>
    public static string Href(MechanismEntry entry) => RoutePrefix + entry.Slug;

    /// <summary>The href for a mechanism page by slug, for the cross-links inside the prose.</summary>
    public static string Href(string slug) => RoutePrefix + slug;

    /// <summary>
    /// The availability line, for a page header that owns its own heading. Read from the same
    /// flag the navigation prints its "WebAssembly only" tag from, so the two cannot drift apart.
    /// </summary>
    public static string Availability(string slug) =>
        Availability(All.First(e => e.Slug == slug).WasmOnly);

    /// <summary>The availability line for a shell that was told the flag directly.</summary>
    public static string Availability(bool wasmOnly) =>
        wasmOnly ? "Blazor WebAssembly only" : "Blazor Server and Blazor WebAssembly";

    /// <summary>The mechanism before this one in article order, or null on the first page.</summary>
    public static MechanismEntry? Previous(string slug)
    {
        for (int i = 0; i < All.Count; i++)
        {
            if (All[i].Slug == slug) return i > 0 ? All[i - 1] : null;
        }
        return null;
    }

    /// <summary>The mechanism after this one in article order, or null on the last page.</summary>
    public static MechanismEntry? Next(string slug)
    {
        for (int i = 0; i < All.Count; i++)
        {
            if (All[i].Slug == slug) return i < All.Count - 1 ? All[i + 1] : null;
        }
        return null;
    }
}
