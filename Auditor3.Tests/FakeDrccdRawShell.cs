using System;
using System.Collections.Generic;
using Auditor3;

namespace Auditor3.Tests;

internal sealed class FakeDrccdRawShell : IDrccdRawShell
{
    private readonly Queue<string> _chunks;

    public FakeDrccdRawShell(
        IEnumerable<string> chunks)
    {
        _chunks = new Queue<string>(
            chunks ?? throw new ArgumentNullException(nameof(chunks)));
    }

    public bool DataAvailable =>
        _chunks.Count > 0;

    public string? LastCommand { get; private set; }

    public bool IsDisposed { get; private set; }

    public string Read()
    {
        if (_chunks.Count == 0)
        {
            return string.Empty;
        }

        return _chunks.Dequeue();
    }

    public void WriteLine(string command)
    {
        LastCommand = command;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }
}
