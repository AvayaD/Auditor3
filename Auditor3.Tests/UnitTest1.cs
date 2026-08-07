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

        var uid = Assert.Single(
            result.Fields,
            field => field.Name == "p_uid");

        Assert.Equal("UID", uid.Type);
        Assert.Equal(12, uid.Offset);
        Assert.Equal(4, uid.Size);
    }
}