using DemoInterop.Server.Components;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Without this, a failing JavaScript -> C# callback is reported as
// "There was an exception invoking 'X'. For more details turn on detailed exceptions..."
// and the demo's whole point is to show the exception the runtime really produced.
builder.Services.Configure<CircuitOptions>(
    options => options.DetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    // The mechanism pages are routable components in the shared Razor class library. Without this,
    // no endpoint exists for them, so pressing F5 on /mechanism/3-in-process hits the status-code
    // pages and lands on /not-found even though the router can render it inside a running circuit.
    .AddAdditionalAssemblies(typeof(DemoInterop.Shared.Components.DemoGallery).Assembly);

app.Run();
