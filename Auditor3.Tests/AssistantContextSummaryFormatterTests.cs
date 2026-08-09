using System;
using System.Collections.Generic;
using Auditor3;

namespace Auditor3.Tests;

public sealed class AssistantContextSummaryFormatterTests
{
    [Fact]
    public void Format_ReturnsContextMetadataAndCounts()
    {
        var context = new AssistantContext
        {
            PrecType = "PR_EXT",
            StructureName = "pr_ext",
            CmRelease = "cm10.2",
            HeaderFile = "cm10.2/pr_ext.ptype",
            CompiledSize = 32,
            DumpSize = 32,
            RecordSizeStatus = "Match",
            Fields = new List<PrecFieldValue>
            {
                new()
                {
                    Name = "p_uid",
                    Type = "UID",
                    Offset = 12,
                    Size = 4
                }
            },
            Evidence = new List<AssistantEvidence>
            {
                new()
                {
                    Type = "CompiledLayout",
                    Content = "Total size: 32 bytes"
                }
            },
            MappingDetails = new List<string>
            {
                "DM_EXT -> PR_EXT"
            },
            ProposedFixes = new List<string>
            {
                "prec pr_ext d ..."
            }
        };

        var result = AssistantContextSummaryFormatter.Format(context);

        Assert.Contains("PREC type        : PR_EXT", result);
        Assert.Contains("Structure        : pr_ext", result);
        Assert.Contains("CM release       : cm10.2", result);
        Assert.Contains(
            "Layout source    : cm10.2/pr_ext.ptype",
            result);
        Assert.Contains("Compiled size    : 32 bytes", result);
        Assert.Contains("Dump size        : 32 bytes", result);
        Assert.Contains("Record status    : Match", result);
        Assert.Contains("Fields           : 1", result);
        Assert.Contains("Evidence         : 1", result);
        Assert.Contains("Mappings         : 1", result);
        Assert.Contains("Proposed fixes   : 1", result);
    }

    [Fact]
    public void Format_UsesUnknownForMissingValues()
    {
        var context = new AssistantContext();

        var result = AssistantContextSummaryFormatter.Format(context);

        Assert.Contains("PREC type        : unknown", result);
        Assert.Contains("Structure        : unknown", result);
        Assert.Contains("CM release       : unknown", result);
        Assert.Contains("Layout source    : unknown", result);
        Assert.Contains("Compiled size    : unknown", result);
        Assert.Contains("Dump size        : unknown", result);
        Assert.Contains("Record status    : Unknown", result);
        Assert.Contains("Fields           : 0", result);
        Assert.Contains("Evidence         : 0", result);
        Assert.Contains("Mappings         : 0", result);
        Assert.Contains("Proposed fixes   : 0", result);
    }

    [Fact]
    public void Format_WithNullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            AssistantContextSummaryFormatter.Format(null!));
    }
}