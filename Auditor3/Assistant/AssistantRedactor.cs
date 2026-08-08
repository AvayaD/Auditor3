using System;
using System.Collections.Generic;

namespace Auditor3;

/// <summary>
/// Applies the initial assistant data policy before context is sent.
/// The first implementation preserves approved structured fields and
/// removes raw evidence content unless explicitly allowed later.
/// </summary>
public sealed class AssistantRedactor : IAssistantRedactor
{
    public AssistantContext Redact(AssistantContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var redactedEvidence = new List<AssistantEvidence>();

        foreach (var evidence in context.Evidence)
        {
            if (evidence is null)
            {
                continue;
            }

            redactedEvidence.Add(new AssistantEvidence
            {
                Type = evidence.Type,
                Source = evidence.Source,
                Description = evidence.Description,
                Content = RedactContent(evidence.Content)
            });
        }

        return new AssistantContext
        {
            ApplicationVersion = context.ApplicationVersion,
            CmRelease = context.CmRelease,
            PrecType = context.PrecType,
            StructureName = context.StructureName,
            HeaderFile = context.HeaderFile,
            StructureSourceLine = context.StructureSourceLine,
            CompiledSize = context.CompiledSize,
            DumpSize = context.DumpSize,
            Fields = context.Fields,
            MappingDetails = context.MappingDetails,
            ProposedFixes = context.ProposedFixes,
            Evidence = redactedEvidence
        };
    }

    private static string RedactContent(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = content.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None);

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];

            if (ContainsSensitiveName(line))
            {
                lines[index] = "[REDACTED]";
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static bool ContainsSensitiveName(string line)
    {
        var value = line.ToLowerInvariant();

        return value.Contains("password")
            || value.Contains("passwd")
            || value.Contains("challenge:")
            || value.Contains("response:")
            || value.Contains("authorization:")
            || value.Contains("bearer ")
            || value.Contains("cookie:")
            || value.Contains("private key")
            || value.Contains("private-key")
            || value.Contains("secret")
            || value.Contains("token:");
    }
}