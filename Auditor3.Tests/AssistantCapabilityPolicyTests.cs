using Auditor3;

namespace Auditor3.Tests;

public sealed class AssistantCapabilityPolicyTests
{
    [Fact]
    public void DisabledMode_AllowsNoCapabilities()
    {
        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.Disabled,
                AssistantCapability.SelectedPrecContext));

        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.Disabled,
                AssistantCapability.PrecLayout));
    }

    [Fact]
    public void OfflineReadOnly_AllowsSuppliedContext()
    {
        Assert.True(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.OfflineReadOnly,
                AssistantCapability.SelectedPrecContext));

        Assert.True(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.OfflineReadOnly,
                AssistantCapability.DeterministicRepairExplanation));
    }

    [Fact]
    public void OfflineReadOnly_DoesNotAllowDrccdCapabilities()
    {
        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.OfflineReadOnly,
                AssistantCapability.PrecLayout));

        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.OfflineReadOnly,
                AssistantCapability.FindPrecsMapping));
    }

    [Fact]
    public void ReadOnly_AllowsInvestigationCapabilities()
    {
        Assert.True(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.ReadOnly,
                AssistantCapability.PrecLayout));

        Assert.True(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.LiveReadOnly,
                AssistantCapability.FindPrecsMapping));
    }

    [Fact]
    public void LabRepairRequiresLabTargetAndApproval()
    {
        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.LabAssisted,
                AssistantCapability.LabRepairExecution,
                engineerApproved: false,
                targetIsDesignatedLab: true));

        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.LabAssisted,
                AssistantCapability.LabRepairExecution,
                engineerApproved: true,
                targetIsDesignatedLab: false));

        Assert.True(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.LabAssisted,
                AssistantCapability.LabRepairExecution,
                engineerApproved: true,
                targetIsDesignatedLab: true));
    }

    [Fact]
    public void LiveReadOnly_DoesNotAllowLabOperations()
    {
        Assert.False(
            AssistantCapabilityPolicy.IsAllowed(
                AssistantExecutionMode.LiveReadOnly,
                AssistantCapability.LabRepairExecution,
                engineerApproved: true,
                targetIsDesignatedLab: true));
    }
}