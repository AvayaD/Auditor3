using System;
using Renci.SshNet;

namespace Auditor3;

/// <summary>
/// Adapts SSH.NET ShellStream to the narrow DRCCD raw-shell boundary.
/// </summary>
internal sealed class DrccdRawShellStream : IDrccdRawShell
{
    private readonly ShellStream _shell;
    private bool _disposed;

    public DrccdRawShellStream(ShellStream shell)
    {
        _shell = shell ??
            throw new ArgumentNullException(nameof(shell));
    }

    public bool DataAvailable
    {
        get
        {
            ThrowIfDisposed();
            return _shell.DataAvailable;
        }
    }

    public string Read()
    {
        ThrowIfDisposed();
        return _shell.Read();
    }

    public void WriteLine(string command)
    {
        ThrowIfDisposed();
        _shell.WriteLine(command);
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
                nameof(DrccdRawShellStream));
        }
    }
}
