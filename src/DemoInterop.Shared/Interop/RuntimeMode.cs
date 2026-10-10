namespace DemoInterop.Shared.Interop;

/// <summary>
/// Which hosting model the assembly is currently running under.
/// </summary>
internal static class RuntimeMode
{
    /// <summary>True when .NET runs in the browser next to JavaScript, i.e. Blazor WebAssembly.</summary>
    public static bool IsWebAssembly => OperatingSystem.IsBrowser();

    public static string Name => IsWebAssembly ? "Blazor WebAssembly" : "Blazor Server";
}
