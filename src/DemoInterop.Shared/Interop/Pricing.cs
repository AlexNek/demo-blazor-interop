using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace DemoInterop.Shared.Interop;

/// <summary>
/// Mechanism 5: a .NET method published to JavaScript by the source generator.
/// WebAssembly only — the Blazor Server app attempts the call so the failure is visible.
/// </summary>
[SupportedOSPlatform("browser")]
public partial class Pricing
{
    [JSExport]
    internal static double ApplyDiscount(double price, int percent)
        => price * (1 - percent / 100d);
}
