using System;

namespace Auditor3.Models
{
    public class AuditResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool Passed { get; set; }
        public string FixScript { get; set; }
        public AuditCategory Category { get; set; }
        public bool RequiresManualFix { get; set; }
        public DateTime AuditDateTime { get; set; } = DateTime.Now;

        public AuditResult(
            string code,
            AuditCategory category,
            bool passed = true,
            string message = "",
            string fixScript = "",
            bool requiresManualFix = false)
        {
            Code = code;
            Category = category;
            Passed = passed;
            Message = message;
            FixScript = fixScript;
            RequiresManualFix = requiresManualFix;
        }

        public override string ToString()
        {
            var status = Passed ? "Passed" : "Failed";
            var manual = RequiresManualFix ? " (Manual Fix)" : "";
            return $"{Code} [{Category}] - {status}{manual}";
        }
    }
}