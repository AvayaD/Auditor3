using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Auditor3;

/// <summary>
/// Read-only WPF interface for asking questions about supplied Auditor3 data.
///
/// This window does not access CM, DRCCD, ToolsA, SSH, SFTP, ShellStream,
/// SAT, TCM, or repair execution.
/// </summary>
public partial class AssistantWindow : Window
{
    private readonly AssistantCoordinator _coordinator;
    private readonly AssistantContext _context;
    private CancellationTokenSource _requestCancellation;

    public AssistantWindow(
        AssistantCoordinator coordinator,
        AssistantContext context)
    {
        _coordinator = coordinator ??
            throw new ArgumentNullException(nameof(coordinator));

        _context = context ??
            throw new ArgumentNullException(nameof(context));

        InitializeComponent();
    }

    private async void Click_Ask(
        object sender,
        RoutedEventArgs args)
    {
        if (_requestCancellation is not null)
        {
            return;
        }

        var question = QuestionBox.Text;

        if (string.IsNullOrWhiteSpace(question))
        {
            StatusText.Text = "Enter a question first.";
            QuestionBox.Focus();
            return;
        }

        _requestCancellation = new CancellationTokenSource();

        SetRequestState(isRequesting: true);
        ResponseBox.Clear();
        StatusText.Text = "Preparing assistant request...";

        try
        {
            var response = await _coordinator.AskAsync(
                question,
                _context,
                cancellationToken: _requestCancellation.Token);

            if (response.Succeeded)
            {
                ResponseBox.Text = BuildResponseText(response);
                StatusText.Text = "Completed. Response is advisory only.";
            }
            else
            {
                ResponseBox.Text = string.Empty;
                StatusText.Text = string.IsNullOrWhiteSpace(
                    response.ErrorMessage)
                    ? "The assistant request failed."
                    : response.ErrorMessage;
            }
        }
        catch (OperationCanceledException)
        {
            ResponseBox.Text = string.Empty;
            StatusText.Text = "Request cancelled.";
        }
        catch (Exception error)
        {
            ResponseBox.Text = string.Empty;
            StatusText.Text =
                $"Assistant request failed: {error.Message}";
        }
        finally
        {
            _requestCancellation.Dispose();
            _requestCancellation = null;
            SetRequestState(isRequesting: false);
        }
    }

    private void Click_Cancel(
        object sender,
        RoutedEventArgs args)
    {
        if (_requestCancellation is null)
        {
            Close();
            return;
        }

        StatusText.Text = "Cancelling request...";
        _requestCancellation.Cancel();
    }

    protected override void OnClosed(EventArgs e)
    {
        _requestCancellation?.Cancel();
        _requestCancellation?.Dispose();
        _requestCancellation = null;

        base.OnClosed(e);
    }

    private void SetRequestState(bool isRequesting)
    {
        AskButton.IsEnabled = !isRequesting;
        CancelButton.IsEnabled = isRequesting;
        QuestionBox.IsEnabled = !isRequesting;
        Cursor = isRequesting
            ? System.Windows.Input.Cursors.Wait
            : System.Windows.Input.Cursors.Arrow;
    }

    private static string BuildResponseText(
        AssistantResponse response)
    {
        var output = new StringBuilder();

        output.AppendLine(
            "ADVISORY AI RESPONSE — VERIFY AGAINST AUDITOR3 RESULTS");
        output.AppendLine();

        output.AppendLine(response.Answer);

        if (response.Warnings.Count > 0)
        {
            output.AppendLine();
            output.AppendLine("Warnings:");

            foreach (var warning in response.Warnings)
            {
                output.AppendLine($"- {warning}");
            }
        }

        if (response.ContainsSuggestedCommands)
        {
            output.AppendLine();
            output.AppendLine(
                "⚠️ This response contains command-like text. " +
                "It is read-only advisory content and was not executed.");
        }

        return output.ToString().TrimEnd();
    }
}