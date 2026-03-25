using Xunit;

namespace Philiprehberger.Disposer.Tests;

public class DisposerExtensionsTests
{
    [Fact]
    public void DisposeWith_RegistersAndReturnsDisposable()
    {
        using var disposer = new Disposer();
        var stream = new MemoryStream();

        var returned = stream.DisposeWith(disposer);

        Assert.Same(stream, returned);
        Assert.Equal(1, disposer.Count);
    }

    [Fact]
    public void DisposeWith_NullDisposer_ThrowsArgumentNullException()
    {
        var stream = new MemoryStream();

        Assert.Throws<ArgumentNullException>(() => stream.DisposeWith(null!));
    }

    [Fact]
    public void DisposeAsyncWith_RegistersAndReturnsDisposable()
    {
        using var disposer = new Disposer();
        var stream = new MemoryStream();

        var returned = stream.DisposeAsyncWith(disposer);

        Assert.Same(stream, returned);
        Assert.Equal(1, disposer.Count);
    }

    [Fact]
    public void DisposeAsyncWith_NullDisposer_ThrowsArgumentNullException()
    {
        var stream = new MemoryStream();

        Assert.Throws<ArgumentNullException>(() => stream.DisposeAsyncWith(null!));
    }
}
