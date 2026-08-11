using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Auditor3;

/// <summary>
/// Controlled adapter over the legacy DRCCD SSH.NET shell stream.
///
/// The underlying shell stream is not exposed to callers. Output is read
/// until the observed DRCCD prompt appears.
/// </summary>
internal sealed class DrccdShellStreamAdapter : IDrccdShell
{
    private static readonly TimeSpan PollInterval =
        TimeSpan.FromMilliseconds(25);

    private readonly ShellStream _shell;
    private bool _disposed;

    public DrccdShellStreamAdapter(ShellStream shell)
    {
        _shell = shell ??
            throw new ArgumentNullException(nameof(shell));
    }

    public void WriteLine(string command)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentException(
                "DRCCD command cannot be empty.",
                nameof(command));
        }

        _shell.WriteLine(command);
    }

    public async Task<string> ReadUntilPromptAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var output = new StringBuilder();
        var start = DateTime.UtcNow;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_shell.DataAvailable)
            {
                var chunk = _shell.Read();

                if (!string.IsNullOrEmpty(chunk))
                {
                    output.Append(chunk);

                    if (DrccdShellPrompt.Pattern.IsMatch(
                            output.ToString()))
                    {
                        return output.ToString();
                    }
                }
            }
            else
            {
                if (DateTime.UtcNow - start >= timeout)
                {
                    throw new TimeoutException(
                        "Timed out waiting for the DRCCD prompt.");
                }

                await Task.Delay(
                        PollInterval,
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            if (DateTime.UtcNow - start >= timeout)
            {
                throw new TimeoutException(
                    "Timed out waiting for the DRCCD prompt.");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _shell.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(
                nameof(DrccdShellStreamAdapter));
        }
    }
}
