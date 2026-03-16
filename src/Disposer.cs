namespace Philiprehberger.Disposer;

/// <summary>
/// Composite disposable container that collects multiple <see cref="IDisposable"/> and
/// <see cref="IAsyncDisposable"/> objects and disposes them all at once in reverse
/// registration order (LIFO) with error aggregation.
/// </summary>
public sealed class Disposer : IDisposable, IAsyncDisposable
{
    private readonly object _lock = new();
    private readonly List<object> _disposables = [];
    private bool _disposed;

    /// <summary>
    /// Gets the number of registered disposables and cleanup actions.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _disposables.Count;
            }
        }
    }

    /// <summary>
    /// Adds one or more <see cref="IDisposable"/> objects to the container.
    /// </summary>
    /// <param name="disposables">The disposable objects to register.</param>
    /// <exception cref="ObjectDisposedException">Thrown if this container has already been disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="disposables"/> is <c>null</c>.</exception>
    public void Add(params IDisposable[] disposables)
    {
        ArgumentNullException.ThrowIfNull(disposables);

        lock (_lock)
        {
            ThrowIfDisposed();

            foreach (var disposable in disposables)
            {
                if (disposable is not null)
                {
                    _disposables.Add(disposable);
                }
            }
        }
    }

    /// <summary>
    /// Adds a cleanup delegate that will be invoked on dispose.
    /// </summary>
    /// <param name="cleanupAction">The cleanup action to register.</param>
    /// <exception cref="ObjectDisposedException">Thrown if this container has already been disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cleanupAction"/> is <c>null</c>.</exception>
    public void Add(Action cleanupAction)
    {
        ArgumentNullException.ThrowIfNull(cleanupAction);

        lock (_lock)
        {
            ThrowIfDisposed();
            _disposables.Add(cleanupAction);
        }
    }

    /// <summary>
    /// Adds an <see cref="IAsyncDisposable"/> object to the container.
    /// </summary>
    /// <param name="disposable">The async disposable object to register.</param>
    /// <exception cref="ObjectDisposedException">Thrown if this container has already been disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="disposable"/> is <c>null</c>.</exception>
    public void AddAsync(IAsyncDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);

        lock (_lock)
        {
            ThrowIfDisposed();
            _disposables.Add(disposable);
        }
    }

    /// <summary>
    /// Disposes all registered objects in reverse registration order (LIFO).
    /// All items are disposed even if some throw; exceptions are collected and
    /// thrown as an <see cref="AggregateException"/>.
    /// </summary>
    public void Dispose()
    {
        List<object> snapshot;

        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;
            snapshot = new List<object>(_disposables);
            _disposables.Clear();
        }

        var exceptions = new List<Exception>();

        for (var i = snapshot.Count - 1; i >= 0; i--)
        {
            try
            {
                switch (snapshot[i])
                {
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                    case Action action:
                        action();
                        break;
                    case IAsyncDisposable asyncDisposable:
                        asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
                        break;
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more errors occurred during disposal.", exceptions);
        }
    }

    /// <summary>
    /// Asynchronously disposes all registered objects in reverse registration order (LIFO).
    /// All items are disposed even if some throw; exceptions are collected and
    /// thrown as an <see cref="AggregateException"/>.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        List<object> snapshot;

        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;
            snapshot = new List<object>(_disposables);
            _disposables.Clear();
        }

        var exceptions = new List<Exception>();

        for (var i = snapshot.Count - 1; i >= 0; i--)
        {
            try
            {
                switch (snapshot[i])
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                    case Action action:
                        action();
                        break;
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more errors occurred during disposal.", exceptions);
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
