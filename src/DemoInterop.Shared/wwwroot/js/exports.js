// Mechanism 5 — JavaScript reaching a [JSExport] method.
// The assembly name and the full type name are part of the JavaScript contract.
// Nothing is caught: on Blazor Server globalThis.getDotnetRuntime does not exist, and the
// demos show that failure rather than hiding it.

export async function callJsExport(price, percent) {
    const dotnetRuntime = globalThis.getDotnetRuntime(0);
    const { getAssemblyExports } = await dotnetRuntime;
    const exports = await getAssemblyExports("DemoInterop.Shared.dll");
    const result = exports.DemoInterop.Shared.Interop.Pricing.ApplyDiscount(price, percent);
    return `${price} minus ${percent}% = ${result}`;
}

// The export path is a string. Rename the type in C# and this line keeps compiling
// while breaking at run time, with nothing on the .NET side to warn you.
export async function callMissingType(price, percent) {
    const dotnetRuntime = globalThis.getDotnetRuntime(0);
    const { getAssemblyExports } = await dotnetRuntime;
    const exports = await getAssemblyExports("DemoInterop.Shared.dll");
    const result = exports.DemoInterop.Shared.Interop.PricingRenamed.ApplyDiscount(price, percent);
    return `UNEXPECTED: ${result}`;
}
