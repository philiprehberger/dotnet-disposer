namespace Philiprehberger.Disposer;

/// <summary>
/// Extension methods for fluent registration of disposable objects with a <see cref="Disposer"/>.
/// </summary>
public static class DisposerExtensions
{
    /// <summary>
    /// Registers the given <see cref="IDisposable"/> with the specified <see cref="Disposer"/>
    /// and returns it for fluent chaining.
    /// </summary>
    /// <typeparam name="T">The type of the disposable object.</typeparam>
    /// <param name="disposable">The disposable object to register.</param>
    /// <param name="disposer">The <see cref="Disposer"/> to register with.</param>
    /// <returns>The original <paramref name="disposable"/> for fluent use.</returns>
    public static T DisposeWith<T>(this T disposable, Disposer disposer) where T : IDisposable
    {
        ArgumentNullException.ThrowIfNull(disposer);
        disposer.Add(disposable);
        return disposable;
    }

    /// <summary>
    /// Registers the given <see cref="IAsyncDisposable"/> with the specified <see cref="Disposer"/>
    /// and returns it for fluent chaining.
    /// </summary>
    /// <typeparam name="T">The type of the async disposable object.</typeparam>
    /// <param name="disposable">The async disposable object to register.</param>
    /// <param name="disposer">The <see cref="Disposer"/> to register with.</param>
    /// <returns>The original <paramref name="disposable"/> for fluent use.</returns>
    public static T DisposeAsyncWith<T>(this T disposable, Disposer disposer) where T : IAsyncDisposable
    {
        ArgumentNullException.ThrowIfNull(disposer);
        disposer.AddAsync(disposable);
        return disposable;
    }
}
