using Auditor3.Models;
using System.Text;

namespace Auditor3.Services
{
    /// <summary>
    /// Base class for all audit strategies
    /// Provides common functionality for audit implementations
    /// </summary>
    public abstract class BaseAuditStrategy : IAuditStrategy
    {
        public abstract string Code { get; }
        public abstract AuditCategory Category { get; }

        /// <summary>
        /// Runs the audit on the given record
        /// </summary>
        public abstract AuditResult Audit(object record);

        /// <summary>
        /// Determines if this strategy can handle the record type
        /// </summary>
        public abstract bool CanHandle(object record);

        /// <summary>
        /// Helper method to create a failure message
        /// </summary>
        protected AuditResult CreateFailure(
            string message,
            string fixScript = "",
            bool requiresManualFix = false)
        {
            return new AuditResult(
                code: Code,
                category: Category,
                passed: false,
                message: message,
                fixScript: fixScript,
                requiresManualFix: requiresManualFix
            );
        }

        /// <summary>
        /// Helper method to create a success result
        /// </summary>
        protected AuditResult CreateSuccess()
        {
            return new AuditResult(
                code: Code,
                category: Category,
                passed: true
            );
        }

        /// <summary>
        /// Helper method to format audit message with fix script for display
        /// </summary>
        protected string FormatMessageWithFix(string auditCode, string issue, string uid, string fixScript)
        {
            var sb = new StringBuilder();
            sb.AppendLine(auditCode);
            sb.AppendLine(issue);
            sb.AppendLine($"UID: {uid}");
            sb.AppendLine("");

            if (!string.IsNullOrEmpty(fixScript) && !fixScript.Contains("MANUAL FIX"))
            {
                sb.AppendLine($"Fix: {fixScript}");
            }
            else if (!string.IsNullOrEmpty(fixScript))
            {
                sb.AppendLine(fixScript);
            }

            sb.AppendLine("");
            return sb.ToString();
        }

        /// <summary>
        /// Helper method to format audit message without fix script
        /// </summary>
        protected string FormatMessage(string auditCode, string issue, string uid)
        {
            var sb = new StringBuilder();
            sb.AppendLine(auditCode);
            sb.AppendLine(issue);
            sb.AppendLine($"UID: {uid}");
            sb.AppendLine("");
            return sb.ToString();
        }

        /// <summary>
        /// Helper method to format message with details
        /// </summary>
        protected string FormatMessage(string auditCode, string issue, params string[] details)
        {
            var sb = new StringBuilder();
            sb.AppendLine(auditCode);
            sb.AppendLine(issue);
            foreach (var detail in details)
            {
                sb.AppendLine(detail);
            }
            sb.AppendLine("");
            return sb.ToString();
        }
    }
}