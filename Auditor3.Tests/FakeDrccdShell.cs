using System;
using System.Threading;
using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

internal sealed class FakeDrccdShell : IDrccdShell
{
    public string? Command { get; private set; }

    public string Output { get; set; } = string.Empty;

    public Exception? ReadError { get; set; }

    public bool IsDisposed { get; private set; }

    public void WriteLine(string command)
    {
        Command = command;
    }

    public Task<string> ReadUntilPromptAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (ReadError is not null)
        {
            throw ReadError;
        }

        return Task.FromResult(Output);
    }

    public void Dispose()
    {
        IsDisposed = true;
    }
}