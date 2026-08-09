using System.Net.Http;
using Auditor3;

namespace Auditor3.Tests;

public sealed class AssistantServiceFactoryTests
{
    [Fact]
    public void DisabledMode_ReturnsDisabledService()
    {
        var service = AssistantServiceFactory.Create(
            AssistantMode.Disabled,
            new AssistantSettings());

        Assert.IsType<DisabledAssistantService>(service);
    }

    [Fact]
    public void LocalMode_ReturnsLocalService()
    {
        var service = AssistantServiceFactory.Create(
            AssistantMode.Local,
            new AssistantSettings
            {
                Enabled = true
            });

        Assert.IsType<LocalAssistantService>(service);
    }

    [Fact]
    public void WebAiMode_WhenDisabled_ReturnsDisabledService()
    {
        using var client = new HttpClient();

        var service = AssistantServiceFactory.Create(
            AssistantMode.WebAI,
            new AssistantSettings
            {
                Enabled = false
            },
            client);

        Assert.IsType<DisabledAssistantService>(service);
    }

    [Fact]
    public void WebAiMode_WhenEnabled_ReturnsWebAiService()
    {
        using var client = new HttpClient();

        var service = AssistantServiceFactory.Create(
            AssistantMode.WebAI,
            new AssistantSettings
            {
                Enabled = true
            },
            client);

        Assert.IsType<WebUiAssistantService>(service);
    }

    [Fact]
    public void NullSettings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            AssistantServiceFactory.Create(
                AssistantMode.Disabled,
                null!));
    }
}