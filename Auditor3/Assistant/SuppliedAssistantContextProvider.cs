using System;

namespace Auditor3;

/// <summary>
/// Supplies a context that was explicitly created by the caller.
///
/// This provider performs no parsing, networking, connection access, or
/// command execution. It is intended as the initial composition seam for
/// the read-only assistant UI.
/// </summary>
public sealed class SuppliedAssistantContextProvider
    : IAssistantContextProvider
{
    private readonly AssistantContext _context;

    public SuppliedAssistantContextProvider(
        AssistantContext context)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
    }

    public bool TryCreateContext(
        out AssistantContext context,
        out string errorMessage)
    {
        context = _context;
        errorMessage = string.Empty;
        return true;
    }
}