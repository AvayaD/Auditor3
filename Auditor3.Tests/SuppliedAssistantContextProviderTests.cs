using Auditor3;

namespace Auditor3.Tests;

public sealed class SuppliedAssistantContextProviderTests
{
    [Fact]
    public void TryCreateContext_ReturnsSuppliedContext()
    {
        var suppliedContext = new AssistantContext
        {
            PrecType = "PR_EXT",
            StructureName = "pr_ext",
            CmRelease = "cm10.2"
        };

        var provider = new SuppliedAssistantContextProvider(
            suppliedContext);

        var succeeded = provider.TryCreateContext(
            out var context,
            out var errorMessage);

        Assert.True(succeeded);
        Assert.Same(suppliedContext, context);
        Assert.Equal(string.Empty, errorMessage);
    }

    [Fact]
    public void Constructor_WithNullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SuppliedAssistantContextProvider(null!));
    }
}