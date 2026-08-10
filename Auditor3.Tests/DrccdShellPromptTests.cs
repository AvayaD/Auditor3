using Auditor3;

namespace Auditor3.Tests;

public sealed class DrccdShellPromptTests
{
    [Theory]
    [InlineData("drccd [44048]->")]
    [InlineData("drccd [44049]->")]
    [InlineData("drccd [44051]->")]
    [InlineData("output\r\ndrccd [44051]->")]
    public void Pattern_MatchesDrccdPrompt(string value)
    {
        Assert.Matches(DrccdShellPrompt.Pattern, value);
    }

    [Theory]
    [InlineData("drccd->")]
    [InlineData("drccd [abc]->")]
    [InlineData("other [44051]->")]
    [InlineData("drccd [44051]")]
    public void Pattern_RejectsInvalidPrompt(string value)
    {
        Assert.DoesNotMatch(DrccdShellPrompt.Pattern, value);
    }
}
