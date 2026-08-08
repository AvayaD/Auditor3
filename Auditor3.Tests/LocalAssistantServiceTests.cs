using System.Threading.Tasks;
using Auditor3;

namespace Auditor3.Tests;

public sealed class LocalAssistantServiceTests
{
    [Fact]
    public async Task AskAsync_MatchingRecord_ReturnsExplanationAndNoWarnings()
    {
        var service = new LocalAssistantService();

        var request = new AssistantRequest
        {
            Question = "Explain this PR_EXT record.",
            CorrelationId = "local-test-001",
            Context = new AssistantContext
            {
                ApplicationVersion = "4.0d",
                CmRelease = "cm10.2",
                PrecType = "PR_EXT",
                StructureName = "pr_ext",
                HeaderFile = "cm10.2/pr_ext.ptype",
                StructureSourceLine = 1525,
                CompiledSize = 32,
                DumpSize = 32,
                RawPrec =
                    "PR_EXT 8aa10004 00000000 00000000 " +
                    "0000971d 00000000 0000ffff 01000000 00000000",
                RecordSizeStatus = "Match",
                Fields =
                [
                    new PrecFieldValue
                    {
                        Name = "ext[8]",
                        Type = "NYBLE",
                        Offset = 2,
                        Size = 8,
                        RawValue = "8aa1",
                        DecodedValue = "1008",
                        DecodeStatus = "Decoded"
                    },
                    new PrecFieldValue
                    {
                        Name = "p_uid",
                        Type = "UID",
                        Offset = 12,
                        Size = 4,
                        RawValue = "0000971d",
                        DecodedValue = "0000971d",
                        DecodeStatus = "Decoded"
                    }
                ],
                Evidence =
                [
                    new AssistantEvidence
                    {
                        Type = "CompiledLayout",
                        Source = "cm10.2/pr_ext.ptype",
                        Description = "Compiled PR_EXT layout",
                        Content = "Total size: 32 bytes"
                    }
                ]
            }
        };

        var response = await service.AskAsync(request);

        Assert.True(response.Succeeded);
        Assert.Equal("local-test-001", response.CorrelationId);
        Assert.False(response.ContainsSuggestedCommands);
        Assert.Empty(response.Warnings);

        Assert.Contains(
            "PREC type: PR_EXT",
            response.Answer);

        Assert.Contains(
            "Structure: pr_ext",
            response.Answer);

        Assert.Contains(
            "Record size status: Match",
            response.Answer);

        Assert.Contains(
            "ext[8]",
            response.Answer);

        Assert.Contains(
            "decoded=1008",
            response.Answer);

        Assert.Contains(
            "p_uid",
            response.Answer);

        Assert.Contains(
            "0000971d",
            response.Answer);

        Assert.Contains(
            "did not contact CM",
            response.Answer);
    }

    [Fact]
    public async Task AskAsync_MismatchedRecord_ReturnsWarning()
    {
        var service = new LocalAssistantService();

        var request = new AssistantRequest
        {
            Question = "Check the record size.",
            CorrelationId = "local-test-002",
            Context = new AssistantContext
            {
                PrecType = "PR_EXT",
                StructureName = "pr_ext",
                CompiledSize = 32,
                DumpSize = 28,
                RecordSizeStatus = "Mismatch"
            }
        };

        var response = await service.AskAsync(request);

        Assert.True(response.Succeeded);
        Assert.Equal("local-test-002", response.CorrelationId);

        Assert.Contains(
            response.Warnings,
            warning => warning.Contains(
                "Raw dump size differs",
                System.StringComparison.Ordinal));

        Assert.Contains(
            "does not match",
            response.Answer);
    }
}