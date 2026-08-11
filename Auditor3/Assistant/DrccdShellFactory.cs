using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Creates a governed, read-only DRCCD shell adapter.
///
/// The existing DRCCD connection must already be connected. This factory
/// does not connect, select a target, or execute a command.
/// </summary>
internal sealed class DrccdShellFactory : IDrccdShellFactory
{
    private readonly DRCCDConnection _connection;

    public DrccdShellFactory(DRCCDConnection connection)
    {
        _connection = connection ??
            throw new ArgumentNullException(nameof(connection));
    }

    public Task<IDrccdShell> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_connection.Connected())
        {
            throw new InvalidOperationException(
                "DRCCD is not connected.");
        }

        var shell = _connection.Shell();

        if (shell is null)
        {
            throw new InvalidOperationException(
                "DRCCD shell could not be created.");
        }

        IDrccdShell adapter =
            new DrccdShellStreamAdapter(shell);

        return Task.FromResult(adapter);
    }
}
