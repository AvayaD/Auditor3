using Auditor3;

namespace Auditor3.Tests;

public sealed class AssistantContextStateTests
{
    [Fact]
    public void DefaultState_IsDisabledAndHasNoContext()
    {
        var state = new AssistantContextState();

        Assert.Equal(
            AssistantMode.Disabled,
            state.Mode);

        Assert.Equal(
            AssistantContextStatus.None,
            state.Status);

        Assert.False(state.HasContext);
        Assert.False(state.IsBusy);
    }

    [Fact]
    public void RequestingState_IsBusy()
    {
        var state = new AssistantContextState
        {
            Mode = AssistantMode.Local,
            Status = AssistantContextStatus.Requesting,
            Context = new AssistantContext
            {
                PrecType = "PR_EXT"
            }
        };

        Assert.True(state.HasContext);
        Assert.True(state.IsBusy);
        Assert.Equal(AssistantMode.Local, state.Mode);
    }

    [Fact]
    public void FailedState_PreservesError()
    {
        var state = new AssistantContextState
        {
            Status = AssistantContextStatus.Failed,
            ErrorMessage = "Assistant request failed."
        };

        Assert.Equal(
            AssistantContextStatus.Failed,
            state.Status);

        Assert.Equal(
            "Assistant request failed.",
            state.ErrorMessage);
    }
}