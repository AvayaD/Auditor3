using Auditor3;

namespace Auditor3.Tests;

public class PrecLayoutParserTests
{
    [Fact]
    public void Parse_PrExtLayout_ReturnsExpectedStructureAndFields()
    {
        const string layout = """
            (gdb) ptype /o struct pr_ext
            /* offset    |  size */  type = struct pr_ext {
            /*    0      |     2 */    short no_digits;
            /*    2      |     8 */    NYBLE ext[8];
            /* XXX  2-byte hole */
            /*   12      |     4 */    UID p_uid;
            /*   16      |     2 */    unsigned short ctbl_idx;
            /* total size (bytes):   32 */
            }
            """;

        var parser = new PrecLayoutParser();

        var result = parser.Parse(
            layout,
            precType: "PR_EXT",
            release: "cm10.2");

        Assert.Equal("PR_EXT", result.PrecType);
        Assert.Equal("pr_ext", result.StructureName);
        Assert.Equal("cm10.2", result.Release);
        Assert.Equal(32, result.TotalSize);

        var noDigits = Assert.Single(
            result.Fields,
            field => field.Name == "no_digits");

        Assert.Equal("short", noDigits.Type);
        Assert.Equal(0, noDigits.Offset);
        Assert.Equal(2, noDigits.Size);

        var extension = Assert.Single(
            result.Fields,
            field => field.Name == "ext[8]");

        Assert.Equal("NYBLE", extension.Type);
        Assert.Equal(2, extension.Offset);
        Assert.Equal(8, extension.Size);

        var uid = Assert.Single(
            result.Fields,
            field => field.Name == "p_uid");

        Assert.Equal("UID", uid.Type);
        Assert.Equal(12, uid.Offset);
        Assert.Equal(4, uid.Size);

        Assert.Contains(
            result.Fields,
            field => field.IsPadding && field.Size == 2);
    }

    [Fact]
    public void Parse_LayoutWithPaddingAndBitField_ReadsMetadata()
    {
        const string layout = """
            (gdb) ptype /o struct example
            /* offset    |  size */  type = struct example {
            /*    0      |     2 */    short first;
            /* XXX  2-byte hole */
            /*    4      |     2 */    struct {
            /*    4:15   |     1 */        unsigned short enabled : 1;
            /*    4:14   |     2 */        unsigned short mode : 2;
                                       } flags;
            /* total size (bytes):    6 */
            }
            """;

        var parser = new PrecLayoutParser();

        var result = parser.Parse(
            layout,
            precType: "EXAMPLE",
            release: "cm10.2");

        Assert.Equal("EXAMPLE", result.PrecType);
        Assert.Equal("example", result.StructureName);
        Assert.Equal("cm10.2", result.Release);
        Assert.Equal(6, result.TotalSize);

        var first = Assert.Single(
            result.Fields,
            field => field.Name == "first");

        Assert.Equal("short", first.Type);
        Assert.Equal(0, first.Offset);
        Assert.Equal(2, first.Size);
        Assert.False(first.IsBitField);

        var padding = Assert.Single(
            result.Fields,
            field => field.IsPadding);

        Assert.Equal(2, padding.Offset);
        Assert.Equal(2, padding.Size);
        Assert.Equal("padding", padding.Type);

        var enabled = Assert.Single(
            result.Fields,
            field => field.Name == "enabled");

        Assert.True(enabled.IsBitField);
        Assert.Equal(4, enabled.Offset);
        Assert.Equal(1, enabled.Size);
        Assert.Equal(15, enabled.BitOffset);
        Assert.Equal(1, enabled.BitSize);

        var mode = Assert.Single(
            result.Fields,
            field => field.Name == "mode");

        Assert.True(mode.IsBitField);
        Assert.Equal(4, mode.Offset);
        Assert.Equal(2, mode.Size);
        Assert.Equal(14, mode.BitOffset);
        Assert.Equal(2, mode.BitSize);
    }

    [Fact]
    public async Task DisabledAssistant_ReturnsDisabledResponse()
    {
        var service = new DisabledAssistantService();

        var request = new AssistantRequest
        {
            Question = "Explain this record.",
            CorrelationId = "test-correlation-id"
        };

        var response = await service.AskAsync(request);

        Assert.False(response.Succeeded);
        Assert.Equal(
            "AI assistant is disabled.",
            response.ErrorMessage);
        Assert.Equal(
            "test-correlation-id",
            response.CorrelationId);
    }

    [Fact]
    public void AssistantRedactor_RedactsSensitiveEvidenceLines()
    {
        var redactor = new AssistantRedactor();

        var context = new AssistantContext
        {
            PrecType = "PR_EXT",
            Evidence =
            [
                new AssistantEvidence
                {
                    Type = "Test",
                    Source = "test",
                    Description = "Sensitive values",
                    Content =
                        "username: engineer\n" +
                        "password: secret-value\n" +
                        "Challenge: 12345\n" +
                        "ordinary line"
                }
            ]
        };

        var result = redactor.Redact(context);

        var evidence = Assert.Single(result.Evidence);

        Assert.Contains("username: engineer", evidence.Content);
        Assert.Contains("ordinary line", evidence.Content);
        Assert.DoesNotContain("secret-value", evidence.Content);
        Assert.DoesNotContain("Challenge: 12345", evidence.Content);
        Assert.Contains("[REDACTED]", evidence.Content);
    }

    [Fact]
    public void AssistantRedactor_PreservesStructuredContext()
    {
        var redactor = new AssistantRedactor();

        var context = new AssistantContext
        {
            ApplicationVersion = "4.0d",
            CmRelease = "cm10.2",
            PrecType = "PR_EXT",
            StructureName = "pr_ext",
            HeaderFile = "dpm_prec.h",
            StructureSourceLine = 1525,
            CompiledSize = 32,
            DumpSize = 32,
            MappingDetails = ["DM_EXT -> PR_EXT"],
            ProposedFixes = ["prec pr_ext d ..."],
            Fields =
            [
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
            ]
        };

        var result = redactor.Redact(context);

        Assert.Equal(context.ApplicationVersion, result.ApplicationVersion);
        Assert.Equal(context.CmRelease, result.CmRelease);
        Assert.Equal(context.PrecType, result.PrecType);
        Assert.Equal(context.StructureName, result.StructureName);
        Assert.Equal(context.HeaderFile, result.HeaderFile);
        Assert.Equal(context.StructureSourceLine, result.StructureSourceLine);
        Assert.Equal(context.CompiledSize, result.CompiledSize);
        Assert.Equal(context.DumpSize, result.DumpSize);
        Assert.Equal(context.MappingDetails, result.MappingDetails);
        Assert.Equal(context.ProposedFixes, result.ProposedFixes);

        var field = Assert.Single(result.Fields);

        Assert.Equal("p_uid", field.Name);
        Assert.Equal("0000971d", field.DecodedValue);
    }
}