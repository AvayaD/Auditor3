using System;
using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class DrccdShellFactoryTests
{
    [Fact]
    public void Constructor_WithNullConnection_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new DrccdShellFactory(null!));
    }

    [Fact]
    public async Task CreateAsync_WhenDisconnected_ThrowsWithoutOpeningShell()
    {
        var connection = new DRCCDConnection();
        var factory = new DrccdShellFactory(connection);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => factory.CreateAsync());

        Assert.Contains(
            "DRCCD is not connected",
            error.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateAsync_CancellationIsCheckedBeforeConnectionAccess()
    {
        var connection = new DRCCDConnection();
        var factory = new DrccdShellFactory(connection);

        using var cancellation =
            new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => factory.CreateAsync(cancellation.Token));
    }
}
