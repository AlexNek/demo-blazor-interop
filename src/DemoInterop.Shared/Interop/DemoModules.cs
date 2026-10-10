using Microsoft.JSInterop;

namespace DemoInterop.Shared.Interop;

/// <summary>
/// Imports a demo's ES module once and keeps the tracked reference, so the component can
/// release it when it goes away. Disposal inside an already-torn-down circuit is swallowed:
/// the handle is gone, and Blazor Server throws instead of failing the component.
/// </summary>
public sealed class DemoModules : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private readonly Dictionary<string, IJSObjectReference> _loaded = [];

    public DemoModules(IJSRuntime js) => _js = js;

    public async ValueTask<IJSObjectReference> LoadAsync(string fileName)
    {
        if (_loaded.TryGetValue(fileName, out var cached))
        {
            return cached;
        }

        var module = await _js.InvokeAsync<IJSObjectReference>("import", DemoAssets.Js(fileName));
        _loaded[fileName] = module;
        return module;
    }

    /// <summary>
    /// The WebAssembly-only form. On Blazor Server the runtime refuses to hand back an
    /// in-process reference, and that exception is what demo 3 shows.
    /// </summary>
    public async ValueTask<IJSInProcessObjectReference> LoadInProcessAsync(string fileName)
    {
        if (_loaded.TryGetValue(fileName, out var cached))
        {
            return (IJSInProcessObjectReference)cached;
        }

        var module = await _js.InvokeAsync<IJSInProcessObjectReference>("import", DemoAssets.Js(fileName));
        _loaded[fileName] = module;
        return module;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var module in _loaded.Values)
        {
            try
            {
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // the circuit is already gone; there is no handle left to release
            }
        }

        _loaded.Clear();
    }
}
