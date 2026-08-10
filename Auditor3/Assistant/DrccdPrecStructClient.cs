using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auditor3;

/// <summary>
/// Retrieves one PREC structure through the governed DRCCD shell boundary.
/// </summary>
public sealed class DrccdPrecStructClient
    : IDrccdPrecStructClient
{
    private readonly IDrccdShellFactory _shellFactory;

    public DrccdPrecStructClient(
        IDrccdShellFactory shellFactory)
    {
        _shellFactory = shellFactory ??
            throw new ArgumentNullException(nameof(shellFactory));
    }

    public async Task<string> GetPrecStructAsync(
        string precType,
        CancellationToken cancellationToken = default)
    {
        if (!DrccdPrecTypeValidator.TryNormalize(
                precType,
                out var normalized))
        {
            throw new ArgumentException(
                "Invalid PREC type.",
                nameof(precType));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var shell = await _shellFactory
            .CreateAsync(cancellationToken)
            .ConfigureAwait(false);

        if (shell is null)
        {
            throw new InvalidOperationException(
                "DRCCD shell could not be created.");
        }

        try
        {
            shell.WriteLine($"./precstruct {normalized}");

            var output = await shell
                .ReadUntilPromptAsync(
                    TimeSpan.FromMinutes(2),
                    cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(output))
            {
                throw new InvalidOperationException(
                    "DRCCD returned empty precstruct output.");
            }

            return output;
        }
        finally
        {
            shell.Dispose();
        }
    }
}