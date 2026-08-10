using System;
using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class DrccdPrecStructClientTests
{
    [Fact]
    public async Task GetPrecStructAsync_NormalizesPrecAndDisposesShell()
    {
        var shell = new FakeDrccdShell
        {
            Output = "precstruct output"
        };

        var factory = new RecordingShellFactory(shell);
        var client = new DrccdPrecStructClient(factory);

        var result = await client.GetPrecStructAsync("PR_EXT");

        Assert.Equal("precstruct output", result);
        Assert.Equal("./precstruct pr_ext", shell.Command);
        Assert.True(shell.IsDisposed);
        Assert.True(factory.WasCalled);
    }

    [Fact]
    public async Task GetPrecStructAsync_RejectsInvalidPrecType()
    {
        var shell = new FakeDrccdShell();
        var factory = new RecordingShellFactory(shell);
        var client = new DrccdPrecStructClient(factory);

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetPrecStructAsync("pr_ext;"));

        Assert.False(factory.WasCalled);
        Assert.Null(shell.Command);
        Assert.False(shell.IsDisposed);
    }

    [Fact]
    public async Task GetPrecStructAsync_EmptyOutputThrowsAndDisposesShell()
    {
        var shell = new FakeDrccdShell
        {
            Output = string.Empty
        };

        var factory = new RecordingShellFactory(shell);
        var client = new DrccdPrecStructClient(factory);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.GetPrecStructAsync("pr_ext"));

        Assert.Equal("./precstruct pr_ext", shell.Command);
        Assert.True(shell.IsDisposed);
    }

    [Fact]
    public async Task GetPrecStructAsync_ClientCancellationBeforeCreation()
    {
        var shell = new FakeDrccdShell
        {
            Output = "precstruct output"
        };

        var factory = new RecordingShellFactory(shell);
        var client = new DrccdPrecStructClient(factory);

        using var cancellation =
            new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => client.GetPrecStructAsync(
                "pr_ext",
                cancellation.Token));

        Assert.False(factory.WasCalled);
        Assert.Null(shell.Command);
        Assert.False(shell.IsDisposed);
    }

    private sealed class RecordingShellFactory
        : IDrccdShellFactory
    {
        private readonly IDrccdShell _shell;

        public RecordingShellFactory(IDrccdShell shell)
        {
            _shell = shell;
        }

        public bool WasCalled { get; private set; }

        public Task<IDrccdShell> CreateAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            WasCalled = true;
            return Task.FromResult(_shell);
        }
    }
}