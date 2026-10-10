# demo-blazor-interop

**Live demo (WebAssembly host, on GitHub Pages): <https://alexnek.github.io/demo-blazor-interop/>**

The companion code for a blog post on JavaScript interop in .NET 10 Blazor. It is a teaching
demo, not a library: eight sections, one per interop mechanism, and each section contains a
green **✓ correct usage** panel and one or more red **✗ wrong usage** panels.

The point of the ✗ panels is that they are not illustrations. Each one makes the real mistake,
catches what the runtime threw, and prints the exception type and message. Nothing is
hand-written error text.

The same component source runs in both hosting models, so you can open the identical page twice
and watch a mechanism work in one window and fail in the other.

## Layout

```
src/
  DemoInterop.Shared/      Razor class library — the 8 demos, the interop types, the ES modules
  DemoInterop.WebAssembly/ Blazor WebAssembly host
  DemoInterop.Server/      Blazor Server host (Razor components, interactive server render mode)
```

`DemoInterop.Shared` is a plain `Microsoft.NET.Sdk.Razor` project targeting `net10.0` with
`AllowUnsafeBlocks`. It is **not** a WebAssembly project, yet it is where `[JSImport]` and
`[JSExport]` live — see [Source generators in a plain net10.0 library](#source-generators-in-a-plain-net100-library).

## Run both hosts

```powershell
dotnet run --project src/DemoInterop.WebAssembly   # http://localhost:5008
dotnet run --project src/DemoInterop.Server        # http://localhost:5162
```

Open both URLs. Each page names the host you are reading it in, at the top.

Keep both in the **Development** environment. On Blazor Server, static web assets from a Razor
class library are served through a development-mode manifest; running the Server app in
Production (for example with `--no-launch-profile`, which is what silently changes the
environment) makes every `_content/DemoInterop.Shared/**` request 500 inside
`StaticAssetDevelopmentRuntimeHandler`, because it then looks for the files in the app's own
`wwwroot`. The symptom looks like a broken demo rather than a wrong environment.

Do not grep the Server page's HTML for `demo.css` and conclude the stylesheet is missing. The
Server host resolves `@Assets[…]` through .NET 10 static asset fingerprinting, so it emits
`_content/DemoInterop.Shared/css/demo.fetnw53u6i.css`, while the WebAssembly `index.html` links the
plain, unfingerprinted path. Both serve the same bytes. The same asymmetry applies to the JS files:
on both hosts the ES modules are reached through an import map, which maps the plain name to a
fingerprinted one, so `import("./_content/DemoInterop.Shared/js/module.js")` still works as written.

## Deploying the WebAssembly host to GitHub Pages

`.github/workflows/deploy-wasm.yml` publishes `DemoInterop.WebAssembly` and uploads the result as a
Pages artifact. The Server host is deliberately not deployed — it needs an ASP.NET Core process
holding a SignalR circuit per client, which static hosting cannot provide.

The published result is <https://alexnek.github.io/demo-blazor-interop/>. Note what that one window
cannot show you: the ✗ panels in sections 3, 5 and 6 exist to display what Blazor *Server* throws, and
the Server host is not deployed, so run it locally to see the other half of the page.

The trap is the base path. A project site is served from `https://<user>.github.io/<repo>/`, while a
fresh `dotnet publish` writes `<base href="/" />` into `index.html`. Everything else in that file is
already relative (`_framework/…`, `_content/…`, `css/app.css`), so rewriting that one attribute is
the whole fix — the workflow does it with a regex and **fails the build** if no `<base>` tag matched,
rather than shipping a site that boots to a blank page.

Two things that look right and are not:

- `-p:StaticWebAssetBasePath` does not touch `<base href>`. It relocates the publish output into a
  nested `wwwroot\<path>\` folder, which is the wrong shape for Pages.
- A rooted asset path such as `/_content/DemoInterop.Shared/js/native.js` resolves against the
  **site** root, so under a project site it loses the `/demo-blazor-interop/` segment and 404s. That
  is why section 6 builds its `JSHost.ImportAsync` URL from `Navigation.BaseUri` instead. Measured in
  a local subdirectory simulation: rooted → 404, base-URI-built → 200 and the call returns.

The workflow also copies the rewritten `index.html` to `404.html`, because Pages has no SPA fallback
and a deep link would otherwise 404 instead of booting Blazor, and it adds `.nojekyll`. Neither file
comes out of `dotnet publish` — verified against a clean publish output.

Before the first run, Pages has to be switched to the **GitHub Actions** source in the repository
settings; the workflow cannot change that itself.

## What each mechanism supports

| # | Mechanism | WebAssembly | Server |
|---|---|---|---|
| 1 | `IJSRuntime` call by global name | yes | yes |
| 2 | ES module import → `IJSObjectReference` | yes | yes |
| 3 | `IJSInProcessObjectReference`, synchronous | yes | **no** |
| 4 | `DotNetObjectReference` + `[JSInvokable]` | yes | yes |
| 5 | `[JSExport]`, JavaScript calling .NET | yes | **no** |
| 6 | `[JSImport]`, checked signature | yes | **no** |
| 7 | Stream references | yes | yes |
| 8 | `InvokeConstructorAsync`, `GetValueAsync` / `SetValueAsync` | yes | yes |

For 3, 5 and 6 the ✓ panel has no button on Blazor Server — the API is not reachable there. The ✗
panel still attempts the call, so the failure is something you can read rather than something you
have to take on trust.

## The failures, as measured

Recorded from this code, not quoted from documentation. The two hosts do not produce the same text.

### 3 — asking the Server runtime for an in-process reference

The code is legal C# and compiles clean on any target. On Blazor Server it fails while
deserialising the return value of `import`:

```
WebAssembly  (no failure — the call works)
Server       JSException: An exception occurred executing JS interop: Deserialization of
             interface or abstract types is not supported. Type
             'Microsoft.JSInterop.IJSInProcessObjectReference'. …
               caused by NotSupportedException: Deserialization of interface or abstract
             types is not supported. Type
             'Microsoft.JSInterop.IJSInProcessObjectReference'.
```

Worth noticing what this is *not*: there is no "this platform does not support that" error. The
runtime never reaches a capability check, it simply cannot materialise the type you asked
`import` to return.

The ✓ panel on WebAssembly also measures the cost of synchronous interop, by the way: a
`requestAnimationFrame` loop records the longest gap between two painted frames while a
synchronous call busy-waits. Measured on one run: the call held the thread **1284 ms**, the
longest painted-frame gap was **1298 ms**. An unblocked page paints every ~16 ms.

### 4 — a JavaScript callback that cannot be dispatched

Same mistake, both hosts, two different wordings for the same .NET parameter:

```
WebAssembly  JSException: System.ArgumentException: There is no tracked object with id '4'.
             Perhaps the DotNetObjectReference instance was already disposed.
             Arg_ParamName_Name, dotNetObjectId
Server       JSException: System.ArgumentException: There is no tracked object with id '4'.
             Perhaps the DotNetObjectReference instance was already disposed.
             (Parameter 'dotNetObjectId')
```

So if you quote one of these strings in writing, quote it as host-specific.

### 5 — reaching a `[JSExport]`

```
WebAssembly  the export runs: 200 minus 15% = 170
Server       JSException: globalThis.getDotnetRuntime is not a function
```

There is no .NET runtime inside the tab on Blazor Server, so the entry point the JavaScript needs
does not exist. The failure is a browser `TypeError`, not a .NET exception.

### 6 — calling through `[JSImport]`

```
WebAssembly  Hello from a module loaded by JSHost.ImportAsync
Server       PlatformNotSupportedException: System.Runtime.InteropServices.JavaScript is
             not supported on this platform.
```

This is the one WASM-only mechanism that fails cleanly, and it fails at the first line of the
method, before any JavaScript is involved.

## Two gotchas that cost real time

### `CircuitOptions.DetailedErrors` is named per circuit

Without detailed errors, every ✗ panel in section 4 reports only:

```
There was an exception invoking 'OnAnimationEnd'. For more details turn on detailed
exceptions in 'CircuitOptions.DetailedErrors'
```

which defeats the purpose of the demo. The fix is the **unnamed** registration:

```csharp
builder.Services.Configure<CircuitOptions>(
    options => options.DetailedErrors = builder.Environment.IsDevelopment());
```

The overload that takes a name — `Configure<CircuitOptions>(builder.Environment.EnvironmentName, …)`,
which is the one commonly copied around — does nothing here. `CircuitOptions` is a named options
type whose name is the **circuit id**, so that call registers configuration for a circuit literally
called `"Development"`, which never exists. It fails silently: no warning, no error, still masked.

### Source generators in a plain net10.0 library

`[JSImport]` and `[JSExport]` do compile and do generate inside the non-browser `net10.0` Razor
class library. Proving it is the awkward part, because an extended partial method with no
implementation still compiles — so a clean build tells you nothing.

```powershell
dotnet build src/DemoInterop.Shared -t:Rebuild -p:EmitCompilerGeneratedFiles=true
```

Look for `JSImports.g.cs` and `JSExports.g.cs` under `obj/`. `-t:Rebuild` is not optional: on an
incremental build the compiler never re-runs, the generator never fires, you get zero generated
files, and the false conclusion is that the setup is broken.

Also note that `<SupportedPlatform Include="browser" />` in the library does **not** produce CA1416
warnings at the Razor-generated call sites here, even with `AnalysisMode=AllEnabledByDefault`. It
was verified with the item and without it: identical output. Do not count on the analyzer to tell
you a call is WebAssembly-only — run the Server build and press the button.

## Contributing

`dotnet build DemoInterop.sln` should stay warning-free. Panels are `RunPanel` components inside a
`DemoShell`; the `Run` parameter is the call itself and exceptions are displayed, not swallowed, so
a demo method should not catch anything.
