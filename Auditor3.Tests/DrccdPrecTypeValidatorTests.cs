using Auditor3;

namespace Auditor3.Tests;

public sealed class DrccdPrecTypeValidatorTests
{
    [Theory]
    [InlineData("PR_EXT", "pr_ext")]
    [InlineData("pr_ext", "pr_ext")]
    [InlineData("Pr_Stn", "pr_stn")]
    [InlineData("PR_AUDIO_GRP", "pr_audio_grp")]
    public void TryNormalize_AcceptsValidPrecNames(
        string input,
        string expected)
    {
        var succeeded =
            DrccdPrecTypeValidator.TryNormalize(
                input,
                out var normalized);

        Assert.True(succeeded);
        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("pr ext")]
    [InlineData("pr-ext")]
    [InlineData("pr.ext")]
    [InlineData("pr/ext")]
    [InlineData("pr_ext;")]
    [InlineData("pr_ext\nother")]
    public void TryNormalize_RejectsInvalidValues(
        string? input)
    {
        var succeeded =
            DrccdPrecTypeValidator.TryNormalize(
                input,
                out var normalized);

        Assert.False(succeeded);
        Assert.Equal(string.Empty, normalized);
    }
}