namespace Auditor3;

/// <summary>
/// Removes or masks sensitive information before assistant data is transmitted.
/// </summary>
public interface IAssistantRedactor
{
    AssistantContext Redact(AssistantContext context);
}