using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Provides deterministic, local explanations from AssistantContext data.
/// This service performs no network operations and does not access CM.
/// </summary>
public sealed class LocalAssistantService : IAssistantService
{
    public Task<AssistantResponse> AskAsync(
        AssistantRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Task.FromResult(new AssistantResponse
            {
                Succeeded = false,
                ErrorMessage = "Assistant request cannot be null."
            });
        }

        cancellationToken.ThrowIfCancellationRequested();

        var context = request.Context;

        if (context is null)
        {
            return Task.FromResult(new AssistantResponse
            {
                Succeeded = false,
                ErrorMessage = "Assistant context cannot be null.",
                CorrelationId = request.CorrelationId
            });
        }

        var answer = BuildExplanation(request.Question, context);

        return Task.FromResult(new AssistantResponse
        {
            Succeeded = true,
            Answer = answer,
            CorrelationId = request.CorrelationId,
            ContainsSuggestedCommands = false,
            Warnings = BuildWarnings(context)
        });
    }

    private static string BuildExplanation(
        string question,
        AssistantContext context)
    {
        var output = new StringBuilder();

        output.AppendLine("## Local Auditor3 Explanation");
        output.AppendLine();

        if (!string.IsNullOrWhiteSpace(question))
        {
            output.AppendLine($"Question: {question}");
            output.AppendLine();
        }

        output.AppendLine("## Authoritative Facts");
        output.AppendLine();

        output.AppendLine($"PREC type: {Display(context.PrecType)}");
        output.AppendLine(
            $"Structure: {Display(context.StructureName)}");
        output.AppendLine(
            $"CM release: {Display(context.CmRelease)}");
        output.AppendLine(
            $"Application version: {Display(context.ApplicationVersion)}");
        output.AppendLine(
            $"Layout source: {Display(context.HeaderFile)}");

        if (context.StructureSourceLine.HasValue)
        {
            output.AppendLine(
                $"Structure source line: {context.StructureSourceLine.Value}");
        }
        else
        {
            output.AppendLine("Structure source line: unknown");
        }

        output.AppendLine(
            $"Compiled size: {FormatSize(context.CompiledSize)}");
        output.AppendLine(
            $"Dump size: {FormatSize(context.DumpSize)}");
        output.AppendLine(
            $"Record size status: {Display(context.RecordSizeStatus)}");

        output.AppendLine();
        output.AppendLine("## Fields");
        output.AppendLine();

        if (context.Fields.Count == 0)
        {
            output.AppendLine("No field values were supplied.");
        }
        else
        {
            foreach (var field in context.Fields)
            {
                output.AppendLine(
                    $"- {field.Name}: " +
                    $"type={field.Type}, " +
                    $"offset={field.Offset}, " +
                    $"size={field.Size}, " +
                    $"raw={Display(field.RawValue)}, " +
                    $"decoded={Display(field.DecodedValue)}, " +
                    $"status={Display(field.DecodeStatus)}");
            }
        }

        output.AppendLine();
        output.AppendLine("## Interpretation");
        output.AppendLine();

        switch (context.RecordSizeStatus)
        {
            case "Match":
                output.AppendLine(
                    "The supplied dump size matches the compiled " +
                    "structure size.");
                break;

            case "Mismatch":
                output.AppendLine(
                    "The supplied dump size does not match the compiled " +
                    "structure size. The record should be investigated " +
                    "before relying on field mappings.");
                break;

            default:
                output.AppendLine(
                    "The record-size comparison is unknown because the " +
                    "compiled size or dump size was not supplied.");
                break;
        }

        output.AppendLine();

        if (context.Evidence.Count == 0)
        {
            output.AppendLine("No supporting evidence was supplied.");
        }
        else
        {
            output.AppendLine(
                $"Supporting evidence items: {context.Evidence.Count}");
        }

        output.AppendLine();
        output.AppendLine("## Advisory");
        output.AppendLine();
        output.AppendLine(
            "This explanation was generated locally from Auditor3 data. " +
            "It did not contact CM or execute any command.");

        return output.ToString().TrimEnd();
    }

    private static IReadOnlyList<string> BuildWarnings(
        AssistantContext context)
    {
        var warnings = new List<string>();

        if (string.Equals(
                context.RecordSizeStatus,
                "Mismatch",
                StringComparison.OrdinalIgnoreCase))
        {
            warnings.Add(
                "Raw dump size differs from compiled structure size.");
        }

        if (!context.CompiledSize.HasValue)
        {
            warnings.Add(
                "Compiled structure size is unavailable.");
        }

        if (!context.DumpSize.HasValue)
        {
            warnings.Add(
                "Raw dump size is unavailable.");
        }

        if (context.Fields.Count == 0)
        {
            warnings.Add(
                "No field values were supplied.");
        }

        return warnings;
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