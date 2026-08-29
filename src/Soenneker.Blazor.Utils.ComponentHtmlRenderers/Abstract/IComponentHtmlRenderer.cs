using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Soenneker.Blazor.Utils.ComponentHtmlRenderers.Abstract;

/// <summary>
/// Renders Blazor components to HTML strings through an underlying <see cref="HtmlRenderer"/>.
/// </summary>
/// <remarks>
/// Implementations handle dispatcher access internally. Typical uses include server-side snapshots,
/// static-site generation, build-time content generation, and component markup tests.
/// </remarks>
public interface IComponentHtmlRenderer : IAsyncDisposable
{
    /// <summary>
    /// Renders a component selected at runtime to an HTML string.
    /// </summary>
    /// <param name="componentType">Component type to render; it must implement <see cref="IComponent"/>.</param>
    /// <param name="parameters">Optional component parameters; <see langword="null"/> renders with an empty parameter set.</param>
    /// <param name="htmlDecode">Whether to HTML-decode the rendered markup before returning it.</param>
    /// <returns>A task whose result is the rendered HTML string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="componentType"/> is <see langword="null"/>.</exception>
    [Pure]
    Task<string> RenderToHtml(Type componentType, IReadOnlyDictionary<string, object?>? parameters = null, bool htmlDecode = false);

    /// <summary>
    /// Renders a component using its generic type as a fast path.
    /// </summary>
    /// <typeparam name="TComponent">Component type to render.</typeparam>
    /// <param name="parameters">Optional component parameters; <see langword="null"/> renders with an empty parameter set.</param>
    /// <param name="htmlDecode">Whether to HTML-decode the rendered markup before returning it.</param>
    /// <returns>A task whose result is the rendered HTML string.</returns>
    /// <remarks>This overload avoids passing a <see cref="Type"/> at runtime.</remarks>
    [Pure]
    Task<string> RenderToHtml<TComponent>(IReadOnlyDictionary<string, object?>? parameters = null, bool htmlDecode = false)
        where TComponent : IComponent;

    /// <summary>
    /// Renders a component selected at runtime, building its parameters inline.
    /// </summary>
    /// <param name="componentType">Component type to render; it must implement <see cref="IComponent"/>.</param>
    /// <param name="buildParameters">Callback that populates the component parameter dictionary.</param>
    /// <param name="htmlDecode">Whether to HTML-decode the rendered markup before returning it.</param>
    /// <returns>A task whose result is the rendered HTML string.</returns>
    /// <remarks>This overload avoids requiring callers to allocate a parameter dictionary before the call.</remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="componentType"/> or <paramref name="buildParameters"/> is <see langword="null"/>.</exception>
    [Pure]
    Task<string> RenderToHtml(Type componentType, Action<Dictionary<string, object?>> buildParameters, bool htmlDecode = false);
}
