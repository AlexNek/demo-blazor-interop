using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace DemoInterop.Shared.Interop;

/// <summary>
/// Mechanism 6: C# calling JavaScript through a build-checked signature.
/// The module name "demo-native" must match the JSHost.ImportAsync registration.
/// WebAssembly only — the Blazor Server app attempts the call so the failure is visible.
/// </summary>
[SupportedOSPlatform("browser")]
public partial class NativeImports
{
    [JSImport("getMessage", "demo-native")]
    internal static partial string GetWelcomeMessage();

    /// <summary>Points at an export that does not exist. Compiles clean, fails when called.</summary>
    [JSImport("noSuchExport", "demo-native")]
    internal static partial string MissingFunction();

    [JSImport("globalThis.console.log")]
    internal static partial void Log([JSMarshalAs<JSType.String>] string message);
}
