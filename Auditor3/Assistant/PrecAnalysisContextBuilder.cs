using System;
using System.Collections.Generic;
using System.Text;

namespace Auditor3;

/// <summary>
/// Builds deterministic AssistantContext objects from compiled PREC layout
/// information and already-decoded field values.
///
/// This class does not access CM, Database, WPF, networking, or Fixer.
/// </summary>
public sealed class PrecAnalysisContextBuilder
    : IPrecAnalysisContextBuilder
{
    public AssistantContext Build(
        PrecLayout layout,
        IReadOnlyList<PrecFieldValue> fields,
        string rawPrec,
        int? dumpSize,
        string cmRelease,
        string applicationVersion)
    {
        if (layout is null)
        {
            throw new ArgumentNullException(nameof(layout));
        }

        if (fields is null)
        {
            throw new ArgumentNullException(nameof(fields));
        }

        var evidence = new List<AssistantEvidence>
        {
            BuildLayoutEvidence(layout)
        };

        if (!string.IsNullOrWhiteSpace(rawPrec))
        {
            evidence.Add(new AssistantEvidence
            {
                Type = "RawPrec",
                Source = layout.SourceFile,
                Description = "Raw PREC record supplied to the context builder.",
                Content = rawPrec
            });
        }

        return new AssistantContext
        {
            ApplicationVersion = applicationVersion ?? string.Empty,
            CmRelease = string.IsNullOrWhiteSpace(cmRelease)
                ? layout.Release
                : cmRelease,
            PrecType = layout.PrecType,
            StructureName = layout.StructureName,
            HeaderFile = layout.SourceFile,
            CompiledSize = layout.TotalSize,
            DumpSize = dumpSize,
            RawPrec = rawPrec ?? string.Empty,
            RecordSizeStatus = GetRecordSizeStatus(
                layout.TotalSize,
                dumpSize),
            Fields = fields,
            Evidence = evidence
        };
    }

    private static string GetRecordSizeStatus(
        int? compiledSize,
        int? dumpSize)
    {
        if (!compiledSize.HasValue || !dumpSize.HasValue)
        {
            return "Unknown";
        }

        return compiledSize.Value == dumpSize.Value
            ? "Match"
            : "Mismatch";
    }

    private static AssistantEvidence BuildLayoutEvidence(
        PrecLayout layout)
    {
        var content = new StringBuilder();

        content.AppendLine(
            $"Structure: struct {layout.StructureName}");

        content.AppendLine(
            $"PREC type: {layout.PrecType}");

        content.AppendLine(
            $"Release: {layout.Release}");

        if (layout.TotalSize.HasValue)
        {
            content.AppendLine(
                $"Total size: {layout.TotalSize.Value} bytes");
        }
        else
        {
            content.AppendLine("Total size: unknown");
        }

        content.AppendLine("Fields:");

        foreach (var field in layout.Fields)
        {
            var category = field.IsPadding
                ? "padding"
                : field.IsBitField
                    ? "bit-field"
                    : "field";

            content.AppendLine(
                $"- {category}: {field.Name}, " +
                $"type={field.Type}, " +
                $"offset={field.Offset}, " +
                $"size={field.Size}");

            if (field.BitOffset.HasValue ||
                field.BitSize.HasValue)
            {
                content.AppendLine(
                    $"  bits: offset={field.BitOffset?.ToString() ?? "unknown"}, " +
                    $"size={field.BitSize?.ToString() ?? "unknown"}");
            }
        }

        return new AssistantEvidence
        {
            Type = "CompiledLayout",
            Source = layout.SourceFile,
            Description = "Compiled PREC structure layout.",
            Content = content.ToString().TrimEnd()
        };
    }
}
