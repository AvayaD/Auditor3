using Auditor3.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auditor3.Services
{
    /// <summary>
    /// Orchestrates audit strategies and produces reports
    /// </summary>
    public class AuditEngine
    {
        private readonly IEnumerable<IAuditStrategy> _strategies;
        private readonly AuditStatistics _statistics;

        public AuditEngine(IEnumerable<IAuditStrategy> strategies)
        {
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
            _statistics = new AuditStatistics();
        }

        /// <summary>
        /// Runs all applicable audits on the given record
        /// </summary>
        public void RunAudit(object record)
        {
            if (record == null) return;

            System.Diagnostics.Debug.WriteLine(
                $"🔍 REFACTORED AUDIT ENGINE: Processing {record.GetType().Name}");

            var applicableStrategies = _strategies.Where(s => s.CanHandle(record));

            foreach (var strategy in applicableStrategies)
            {
                var result = strategy.Audit(record);
                _statistics.AddResult(result);
            }
        }

        /// <summary>
        /// Runs all audits on a collection of records
        /// </summary>
        public void RunAudits(IEnumerable<object> records)
        {
            foreach (var record in records ?? Enumerable.Empty<object>())
            {
                RunAudit(record);
            }
        }

        /// <summary>
        /// Gets the audit statistics
        /// </summary>
        public AuditStatistics GetStatistics() => _statistics;

        /// <summary>
        /// Generates a formatted report
        /// </summary>
        public string GenerateReport() => _statistics.GenerateReport();

        /// <summary>
        /// Resets all statistics
        /// </summary>
        public void Reset() => _statistics.Reset();
    }
}