using System;
using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class DrccdShellStreamAdapterTests
{
    [Fact]
    public async Task ReadUntilPromptAsync_CombinesChunksUntilDrccdPrompt()
    {
        var rawShell = new FakeDrccdRawShell(
            [
                "?? Deriving file paths dynamically...\r\n",
                "Layout file: /home/test/pr_ext.ptype\r\n",
                "drccd [44051]->"
            ]);

        using var adapter =
            new DrccdShellStreamAdapter(rawShell);

        var output = await adapter.ReadUntilPromptAsync(
            TimeSpan.FromSeconds(1));

        Assert.Contains(
            "?? Deriving file paths dynamically...",
            output);

        Assert.Contains(
            "Layout file: /home/test/pr_ext.ptype",
            output);

        Assert.EndsWith(
            "drccd [44051]->",
            output,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteLine_ForwardsCommandToRawShell()
    {
        var rawShell = new FakeDrccdRawShell([]);

        using var adapter =
            new DrccdShellStreamAdapter(rawShell);

        adapter.WriteLine("./precstruct pr_ext");

        Assert.Equal(
            "./precstruct pr_ext",
            rawShell.LastCommand);
    }

    [Fact]
    public void WriteLine_RejectsEmptyCommand()
    {
        var rawShell = new FakeDrccdRawShell([]);

        using var adapter =
            new DrccdShellStreamAdapter(rawShell);

        Assert.Throws<ArgumentException>(
            () => adapter.WriteLine(" "));
    }

    [Fact]
    public async Task ReadUntilPromptAsync_WithoutPromptTimesOut()
    {
        var rawShell = new FakeDrccdRawShell(
            ["partial output\r\n"]);

        using var adapter =
            new DrccdShellStreamAdapter(rawShell);

        await Assert.ThrowsAsync<TimeoutException>(
            () => adapter.ReadUntilPromptAsync(
                TimeSpan.FromMilliseconds(75)));
    }

    [Fact]
    public async Task ReadUntilPromptAsync_HonorsCancellation()
    {
        var rawShell = new FakeDrccdRawShell([]);

        using var adapter =
            new DrccdShellStreamAdapter(rawShell);

        using var cancellation =
            new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => adapter.ReadUntilPromptAsync(
                TimeSpan.FromSeconds(1),
                cancellation.Token));
    }

    [Fact]
    public void Dispose_ForwardsDisposalAndIsIdempotent()
    {
        var rawShell = new FakeDrccdRawShell([]);

        var adapter =
            new DrccdShellStreamAdapter(rawShell);

        adapter.Dispose();
        adapter.Dispose();

        Assert.True(rawShell.IsDisposed);
    }

    [Fact]
    public void UseAfterDispose_Throws()
    {
        var rawShell = new FakeDrccdRawShell([]);

        var adapter =
            new DrccdShellStreamAdapter(rawShell);

        adapter.Dispose();

        Assert.Throws<ObjectDisposedException>(
            () => adapter.WriteLine("./precstruct pr_ext"));
    }
}
