using System;
using System.Net.Http;

namespace Auditor3;

/// <summary>
/// Creates the selected read-only assistant implementation.
/// </summary>
public static class AssistantServiceFactory
{
    public static IAssistantService Create(
        AssistantMode mode,
        AssistantSettings settings,
        HttpClient httpClient = null)
    {
        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        switch (mode)
        {
            case AssistantMode.Local:
                return new LocalAssistantService();

            case AssistantMode.WebAI:
                if (!settings.Enabled)
                {
                    return new DisabledAssistantService();
                }

                return new WebUiAssistantService(
                    httpClient ?? new HttpClient(),
                    settings);

            case AssistantMode.Disabled:
            default:
                return new DisabledAssistantService();
        }
    }
}