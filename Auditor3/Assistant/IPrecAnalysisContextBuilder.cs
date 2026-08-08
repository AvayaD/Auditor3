using System.Collections.Generic;

namespace Auditor3;

/// <summary>
/// Builds deterministic assistant context from PREC layout and field data.
/// This interface must remain independent of networking, WPF, and CM connections.
/// </summary>
public interface IPrecAnalysisContextBuilder
{
    AssistantContext Build(
        PrecLayout layout,
        IReadOnlyList<PrecFieldValue> fields,
        string rawPrec,
        int? dumpSize,
        string cmRelease,
        string applicationVersion);
}
