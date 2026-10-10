# demo-blazor-interop

**Live demo:** [Blazor WebAssembly](https://alexnek.github.io/demo-blazor-interop/) · [Blazor Server](https://demointeropserver2026-dpfsdvbacygvgqey.francecentral-01.azurewebsites.net/)

The companion code for a blog post on JavaScript interop in .NET 10 Blazor. It is a teaching
demo, not a library: eight sections, one per interop mechanism, and each section contains a
green **✓ correct usage** panel and one or more red **✗ wrong usage** panels.

The ✗ panels are not illustrations: each one makes the real mistake and prints the exception the
runtime actually threw.

The same component source runs in both hosting models, so you can open the identical page twice
and watch a mechanism work in one window and fail in the other.

There are two views of the eight mechanisms: one scrolling page that holds them all, and one page per
mechanism that goes deeper on that mechanism alone. Both hosts serve both views.

## Layout

```
src/
  DemoInterop.Shared/      Razor class library — the 8 demos, the interop types, the ES modules
  DemoInterop.WebAssembly/ Blazor WebAssembly host
  DemoInterop.Server/      Blazor Server host (Razor components, interactive server render mode)
```

## Run both hosts

```powershell
dotnet run --project src/DemoInterop.WebAssembly   # http://localhost:5008
dotnet run --project src/DemoInterop.Server        # http://localhost:5162
```

Open both URLs. Every page names the host you are reading it in, at the top, and links to the same
page on the other host's deployment, so the two can be opened side by side.

## The eight mechanisms

| # | Mechanism | Route | WebAssembly | Server |
|---|---|---|---|---|
| 1 | `IJSRuntime` call by global name | `mechanism/1-ijsruntime` | yes | yes |
| 2 | ES module import → `IJSObjectReference` | `mechanism/2-es-modules` | yes | yes |
| 3 | `IJSInProcessObjectReference`, synchronous | `mechanism/3-in-process` | yes | **no** |
| 4 | `DotNetObjectReference` + `[JSInvokable]` | `mechanism/4-dotnet-object-reference` | yes | yes |
| 5 | `[JSExport]`, JavaScript calling .NET | `mechanism/5-jsexport` | yes | **no** |
| 6 | `[JSImport]`, checked signature | `mechanism/6-jsimport` | yes | **no** |
| 7 | Stream references | `mechanism/7-streams` | yes | yes |
| 8 | `InvokeConstructorAsync`, `GetValueAsync` / `SetValueAsync` | `mechanism/8-dotnet-10-additions` | yes | yes |

For 3, 5 and 6 the ✓ panel has no button on Blazor Server — the API is not reachable there. The ✗
panel still attempts the call, so you can read what it throws there.

## License

GPL-3.0 — see [LICENSE](LICENSE).
