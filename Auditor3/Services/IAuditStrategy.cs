using Auditor3.Models;

namespace Auditor3.Services
{
    public interface IAuditStrategy
    {
        string Code { get; }
        AuditCategory Category { get; }
        AuditResult Audit(object record);
        bool CanHandle(object record);
    }
}