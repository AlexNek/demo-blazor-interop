namespace DemoInterop.Shared.Interop;

/// <summary>
/// One place that knows where the JavaScript assets of this class library are served from.
/// Static web assets of a Razor class library live under _content/{AssemblyName}/.
/// </summary>
internal static class DemoAssets
{
    public const string ContentRoot = "./_content/DemoInterop.Shared";

    public static string Js(string fileName) => $"{ContentRoot}/js/{fileName}";

    /// <summary>
    /// The same file as an absolute URL, for the one API that needs one: <c>JSHost.ImportAsync</c>.
    /// A path starting with <c>/</c> is resolved against the <em>site</em> root, so it breaks as soon
    /// as the app is hosted under a subdirectory — which is how a GitHub Pages project site serves
    /// it, at <c>https://&lt;user&gt;.github.io/&lt;repo&gt;/</c>. Measured on WebAssembly in both
    /// shapes: the rooted path 404s under the subdirectory, the base-URI-built one loads and the
    /// call returns.
    /// </summary>
    public static string JsAbsolute(string baseUri, string fileName) => $"{baseUri}{Js(fileName)[2..]}";

    /// <summary>The module name a [JSImport] attribute and the JSHost.ImportAsync call must agree on.</summary>
    public const string NativeModuleName = "demo-native";
}
