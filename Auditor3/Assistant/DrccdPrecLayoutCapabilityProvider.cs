using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Provides compiled PREC layout information retrieved through the approved
/// DRCCD client boundary.
///
/// This provider does not execute shell commands directly.
/// </summary>
public sealed class DrccdPrecLayoutCapabilityProvider
    : IAssistantCapabilityProvider
{
    private readonly IDrccdPrecStructClient _client;
    private readonly PrecLayoutParser _parser;

    public DrccdPrecLayoutCapabilityProvider(
        IDrccdPrecStructClient client,
        PrecLayoutParser parser = null)
    {
        _client = client ??
            throw new ArgumentNullException(nameof(client));

        _parser = parser ?? new PrecLayoutParser();
    }

    public bool CanHandle(AssistantCapability capability)
    {
        return capability == AssistantCapability.PrecLayout;
    }

    public async Task<AssistantCapabilityResult> HandleAsync(
        AssistantCapabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Failure(
                "Capability request cannot be null.",
                string.Empty);
        }

        if (string.IsNullOrWhiteSpace(request.PrecType))
        {
            return Failure(
                "PREC type is required for a layout request.",
                request.CorrelationId);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var precType = request.PrecType.Trim();
        string output;

        try
        {
            output = await _client
                .GetPrecStructAsync(
                    precType,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception error)
        {
            return Failure(
                $"DRCCD precstruct retrieval failed: {error.Message}",
                request.CorrelationId);
        }

        if (string.IsNullOrWhiteSpace(output))
        {
            return Failure(
                "DRCCD returned no precstruct output.",
                request.CorrelationId);
        }

        try
        {
            var layout = _parser.Parse(
                output,
                precType: precType);

            var context = new AssistantContext
            {
                PrecType = layout.PrecType,
                StructureName = layout.StructureName,
                HeaderFile = layout.SourceFile,
                CompiledSize = layout.TotalSize,
                RecordSizeStatus = "Unknown",
                Evidence = new List<AssistantEvidence>
                {
                    new AssistantEvidence
                    {
                        Type = "CompiledLayout",
                        Source = layout.SourceFile,
                        Description =
                            "Compiled PREC layout retrieved from DRCCD.",
                        Content = BuildLayoutSummary(layout)
                    },
                    new AssistantEvidence
                    {
                        Type = "PrecStructOutput",
                        Source = "DRCCD ./precstruct",
                        Description =
                            "Raw source declaration and GDB layout output.",
                        Content = output
                    }
                }
            };

            return new AssistantCapabilityResult
            {
                Capability = AssistantCapability.PrecLayout,
                Succeeded = true,
                Context = context,
                Summary =
                    $"Retrieved compiled layout for {layout.PrecType}.",
                CorrelationId = request.CorrelationId
            };
        }
        catch (Exception error)
        {
            return Failure(
                $"Could not parse DRCCD precstruct output: {error.Message}",
                request.CorrelationId);
        }
    }

    private static string BuildLayoutSummary(PrecLayout layout)
    {
        var output = new StringBuilder();

        output.AppendLine(
            $"Structure: struct {layout.StructureName}");
        output.AppendLine(
            $"PREC type: {layout.PrecType}");
        output.AppendLine(
            $"Release: {Display(layout.Release)}");
        output.AppendLine(
            $"Total size: {FormatSize(layout.TotalSize)}");
        output.AppendLine("Fields:");

        foreach (var field in layout.Fields)
        {
            var category = field.IsPadding
                ? "padding"
                : field.IsBitField
                    ? "bit-field"
                    : "field";

            output.AppendLine(
                $"- {category}: {field.Name}, " +
                $"type={field.Type}, " +
                $"offset={field.Offset}, " +
                $"size={field.Size}");
        }

        return output.ToString().TrimEnd();
    }

    private static AssistantCapabilityResult Failure(
        string message,
        string correlationId)
    {
        return new AssistantCapabilityResult
        {
            Capability = AssistantCapability.PrecLayout,
            Succeeded = false,
            ErrorMessage = message,
            CorrelationId = correlationId
        };
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