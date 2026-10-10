namespace DemoInterop.Shared.Interop;

/// <summary>
/// Where each hosting model is published. Both deployments serve the identical set of routes, each
/// under its own base URL, which is what lets a page in one host link to the same page in the other.
/// </summary>
internal static class HostSites
{
    /// <summary>WebAssembly host on GitHub Pages. Ends with a slash because it is served from a subdirectory.</summary>
    public const string WebAssemblyBase = "https://alexnek.github.io/demo-blazor-interop/";

    /// <summary>Server host on Azure App Service, served from the site root.</summary>
    public const string ServerBase = "https://demointeropserver2026-dpfsdvbacygvgqey.francecentral-01.azurewebsites.net/";

    /// <summary>
    /// The address of the page at <paramref name="currentUri"/> on the hosting model this process is
    /// <em>not</em> running in. This app's own <paramref name="baseUri"/> is stripped so the route that
    /// is left can be re-rooted onto the other host; a URL outside the base falls back to that host's
    /// root. The point is that a comparison link lands on the same page, not on the other host's
    /// front page.
    /// </summary>
    public static string Counterpart(string currentUri, string baseUri)
    {
        var path = currentUri.StartsWith(baseUri, StringComparison.Ordinal)
            ? currentUri[baseUri.Length..]
            : string.Empty;
        var otherBase = RuntimeMode.IsWebAssembly ? ServerBase : WebAssemblyBase;
        return otherBase + path.TrimStart('/');
    }
}
