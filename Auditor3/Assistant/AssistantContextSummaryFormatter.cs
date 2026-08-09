using System;
using System.Text;

namespace Auditor3;

/// <summary>
/// Formats a deterministic, read-only summary of assistant context.
///
/// This class has no WPF, networking, CM, SSH, shell, SAT, TCM, or repair
/// dependencies.
/// </summary>
public static class AssistantContextSummaryFormatter
{
    public static string Format(AssistantContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var output = new StringBuilder();

        output.AppendLine("Assistant Context");
        output.AppendLine("=================");
        output.AppendLine(
            $"PREC type        : {Display(context.PrecType)}");
        output.AppendLine(
            $"Structure        : {Display(context.StructureName)}");
        output.AppendLine(
            $"CM release       : {Display(context.CmRelease)}");
        output.AppendLine(
            $"Layout source    : {Display(context.HeaderFile)}");
        output.AppendLine(
            $"Compiled size    : {FormatSize(context.CompiledSize)}");
        output.AppendLine(
            $"Dump size        : {FormatSize(context.DumpSize)}");
        output.AppendLine(
            $"Record status    : {Display(context.RecordSizeStatus)}");
        output.AppendLine(
            $"Fields           : {context.Fields.Count}");
        output.AppendLine(
            $"Evidence         : {context.Evidence.Count}");
        output.AppendLine(
            $"Mappings         : {context.MappingDetails.Count}");
        output.AppendLine(
            $"Proposed fixes   : {context.ProposedFixes.Count}");

        return output.ToString().TrimEnd();
    }

    private static string FormatSize(int? size)
    {
        return size.HasValue
            ? $"{size.Value} bytes"
            : "unknown";
    }

    private static string Display(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "unknown"
            : value;
    }
}