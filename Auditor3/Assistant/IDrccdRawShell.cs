using System;

namespace Auditor3;

/// <summary>
/// Minimal internal abstraction over the SSH.NET shell operations required by
/// the governed DRCCD adapter.
///
/// This interface is not exposed to the assistant and does not provide
/// arbitrary shell access.
/// </summary>
internal interface IDrccdRawShell : IDisposable
{
    bool DataAvailable { get; }

    string Read();

    void WriteLine(string command);
}
