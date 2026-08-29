# Soenneker.Blazor.Utils.ComponentHtmlRenderers

Renders Blazor components to HTML strings through an underlying `HtmlRenderer`.

## Install

```bash
dotnet add package Soenneker.Blazor.Utils.ComponentHtmlRenderers
```

## Quick start

```csharp
using Soenneker.Blazor.Utils.ComponentHtmlRenderers.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddComponentHtmlRendererAsSingleton();
```

Adds `IComponentHtmlRenderer` as a singleton service.

## What you get

- `IComponentHtmlRenderer` — Renders Blazor components to HTML strings through an underlying `HtmlRenderer`.
- `ComponentHtmlRendererRegistrar` — A headless Blazor renderer.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IComponentHtmlRenderer.RenderToHtml(componentType, parameters, htmlDecode)` | Renders a component selected at runtime to an HTML string. | A task whose result is the rendered HTML string. |
| `IComponentHtmlRenderer.RenderToHtml(parameters, htmlDecode)` | Renders a component using its generic type as a fast path. | A task whose result is the rendered HTML string. |
| `IComponentHtmlRenderer.RenderToHtml(componentType, buildParameters, htmlDecode)` | Renders a component selected at runtime, building its parameters inline. | A task whose result is the rendered HTML string. |
| `ComponentHtmlRendererRegistrar.AddComponentHtmlRendererAsSingleton(services)` | Adds `IComponentHtmlRenderer` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `ComponentHtmlRendererRegistrar.AddComponentHtmlRendererAsScoped(services)` | Adds `IComponentHtmlRenderer` as a scoped service. | The same service collection, so additional registrations can be chained. |

## Important behavior

- `IComponentHtmlRenderer`: Implementations handle dispatcher access internally. Typical uses include server-side snapshots, static-site generation, build-time content generation, and component markup tests.
- `IComponentHtmlRenderer.RenderToHtml(componentType, parameters, htmlDecode)`: Thrown when `componentType` is `null`.
- `IComponentHtmlRenderer.RenderToHtml(parameters, htmlDecode)`: This overload avoids passing a `Type` at runtime.
- `IComponentHtmlRenderer.RenderToHtml(componentType, buildParameters, htmlDecode)`: This overload avoids requiring callers to allocate a parameter dictionary before the call.
- `IComponentHtmlRenderer.RenderToHtml(componentType, buildParameters, htmlDecode)`: Thrown when `componentType` or `buildParameters` is `null`.

## Practical notes

- Dispose instances you own when their scope ends so held resources can be released.
