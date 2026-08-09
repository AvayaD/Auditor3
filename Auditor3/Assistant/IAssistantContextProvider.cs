namespace Auditor3;

/// <summary>
/// Supplies a deterministic assistant context for the current UI selection.
///
/// Implementations must build context from already-available Auditor3 data.
/// They must not open connections, execute commands, or access CM, DRCCD,
/// ToolsA, SSH, SFTP, ShellStream, SAT, TCM, or repair execution.
/// </summary>
public interface IAssistantContextProvider
{
    /// <summary>
    /// Attempts to create the context that will be shown to the assistant.
    /// </summary>
    bool TryCreateContext(
        out AssistantContext context,
        out string errorMessage);
}