using Xunit;

namespace Philiprehberger.Disposer.Tests;

public class DisposerTests
{
    [Fact]
    public void Add_Disposable_IncrementsCount()
    {
        using var disposer = new Disposer();
        var stream = new MemoryStream();

        disposer.Add(stream);

        Assert.Equal(1, disposer.Count);
    }

    [Fact]
    public void Dispose_DisposesAllInReverseOrder()
    {
        var order = new List<int>();
        var disposer = new Disposer();

        disposer.Add(() => order.Add(1));
        disposer.Add(() => order.Add(2));
        disposer.Add(() => order.Add(3));

        disposer.Dispose();

        Assert.Equal(new[] { 3, 2, 1 }, order);
    }

    [Fact]
    public void Dispose_AlreadyDisposed_DoesNothing()
    {
        var callCount = 0;
        var disposer = new Disposer();
        disposer.Add(() => callCount++);

        disposer.Dispose();
        disposer.Dispose();

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Add_AfterDispose_ThrowsObjectDisposedException()
    {
        var disposer = new Disposer();
        disposer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => disposer.Add(new MemoryStream()));
    }

    [Fact]
    public void Dispose_WithErrors_ThrowsAggregateException()
    {
        var disposer = new Disposer();
        disposer.Add(() => throw new InvalidOperationException("test"));

        Assert.Throws<AggregateException>(() => disposer.Dispose());
    }

    [Fact]
    public async Task DisposeAsync_DisposesAllInReverseOrder()
    {
        var order = new List<int>();
        var disposer = new Disposer();

        disposer.Add(() => order.Add(1));
        disposer.Add(() => order.Add(2));
        disposer.Add(() => order.Add(3));

        await disposer.DisposeAsync();

        Assert.Equal(new[] { 3, 2, 1 }, order);
    }

    [Fact]
    public void Add_CleanupAction_IncrementsCount()
    {
        using var disposer = new Disposer();

        disposer.Add(() => { });
        disposer.Add(() => { });

        Assert.Equal(2, disposer.Count);
    }
}
