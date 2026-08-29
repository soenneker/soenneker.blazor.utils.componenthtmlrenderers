[![](https://img.shields.io/nuget/v/soenneker.blazor.utils.componenthtmlrenderers.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.utils.componenthtmlrenderers/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.utils.componenthtmlrenderers/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.utils.componenthtmlrenderers/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.utils.componenthtmlrenderers.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.utils.componenthtmlrenderers/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.utils.componenthtmlrenderers/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.utils.componenthtmlrenderers/actions/workflows/codeql.yml)

# Soenneker.Blazor.Utils.ComponentHtmlRenderers

Renders a Blazor component to a static HTML string outside an interactive Blazor circuit.

Use it for emails, snapshots, static generation, or build-time markup inspection. The result contains no live event handling, circuit, or browser-side interactivity.

## Installation

```bash
dotnet add package Soenneker.Blazor.Utils.ComponentHtmlRenderers
```

Register the renderer with the same service lifetime as the dependencies used by your components. Scoped registration is the usual choice in an ASP.NET Core application:

```csharp
using Soenneker.Blazor.Utils.ComponentHtmlRenderers.Registrars;

builder.Services.AddComponentHtmlRendererAsScoped();
```

```csharp
public sealed class ReceiptRenderer(IComponentHtmlRenderer renderer)
{
    public Task<string> Render(string customerName, decimal total)
    {
        return renderer.RenderToHtml<Receipt>(new Dictionary<string, object?>
        {
            [nameof(Receipt.CustomerName)] = customerName,
            [nameof(Receipt.Total)] = total
        });
    }
}
```

Parameter names and values follow normal Blazor component parameter binding. Invalid names or incompatible values fail during rendering.

## Build parameters inline

Use the runtime-type overload when the component type is selected dynamically:

```csharp
string html = await renderer.RenderToHtml(componentType, parameters =>
{
    parameters["Title"] = "Weekly summary";
    parameters["Items"] = reportItems;
});
```

Passing a read-only dictionary is also supported:

```csharp
string html = await renderer.RenderToHtml(
    typeof(Receipt),
    new Dictionary<string, object?>
    {
        [nameof(Receipt.CustomerName)] = "Ada"
    });
```

Rendering is dispatched through Blazor’s renderer dispatcher and waits for the component hierarchy to become quiescent, including asynchronous initialization. Concurrent callers are serialized by that dispatcher.

## Standalone use

For a console tool or test without an existing application service provider, construct and dispose the renderer directly. Logging is required, along with any services injected by the component tree:

```csharp
await using var renderer = new ComponentHtmlRenderer(services =>
{
    services.AddLogging();
    services.AddSingleton<IPriceFormatter, PriceFormatter>();
});

string html = await renderer.RenderToHtml<Receipt>();
```

The constructor owns and disposes the service provider it creates by default. When passing an existing `IServiceProvider`, the renderer does not dispose it unless explicitly requested.

## Encoding and trust boundaries

Normal rendering leaves Blazor’s HTML encoding intact. Keep the default `htmlDecode: false` for output that will be stored, emailed, or served:

```csharp
string safeMarkup = await renderer.RenderToHtml<Receipt>();
```

`htmlDecode: true` decodes the entire rendered string. It exists for trusted build-time text processing, such as scanning Tailwind arbitrary variants containing encoded ampersands. Decoding can turn encoded user content into executable markup, so never enable it for output that reaches a browser or email client.

Components execute server-side during rendering and can access their injected services. Treat the component type and parameter values as trusted application inputs, and do not expose arbitrary type selection to users.

## Service lifetime

Use scoped registration when rendered components consume scoped dependencies. Singleton registration is appropriate only when every dependency reachable from the rendered component tree is safe to resolve from the root provider and to share for the application lifetime:

```csharp
builder.Services.AddComponentHtmlRendererAsSingleton();
```

The DI container disposes registered renderer instances. Dispose manually constructed instances with `await using`.
