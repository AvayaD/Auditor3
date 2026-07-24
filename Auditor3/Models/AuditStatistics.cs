using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditor3.Models
{
    public class AuditStatistics
    {
        public int TotalCorrupted { get; private set; }
        public int CorruptedStations { get; private set; }
        public int CorruptedTrunks { get; private set; }
        public int CorruptedAnnouncements { get; private set; }
        public int ManualFixes { get; private set; }
        public int TotalChecked { get; private set; }
        public int TotalToCheck { get; private set; }

        private readonly Dictionary<string, int> _auditHits = new Dictionary<string, int>();
        public List<AuditResult> Results { get; } = new List<AuditResult>();

        public void RecordFailure(
            string auditCode,
            AuditCategory category,
            bool requiresManualFix = false)
        {
            TotalCorrupted++;

            switch (category)
            {
                case AuditCategory.Station:
                    CorruptedStations++;
                    break;
                case AuditCategory.Trunk:
                    CorruptedTrunks++;
                    break;
                case AuditCategory.Announcement:
                    CorruptedAnnouncements++;
                    break;
            }

            if (requiresManualFix)
                ManualFixes++;

            if (!_auditHits.ContainsKey(auditCode))
                _auditHits[auditCode] = 0;
            _auditHits[auditCode]++;
        }

        public void AddResult(AuditResult result)
        {
            if (result == null) return;
            Results.Add(result);
            if (!result.Passed)
            {
                RecordFailure(result.Code, result.Category, result.RequiresManualFix);
            }
        }

        public int GetAuditHitCount(string auditCode)
        {
            return _auditHits.ContainsKey(auditCode) ? _auditHits[auditCode] : 0;
        }

        public IReadOnlyDictionary<string, int> GetAllAuditHits()
        {
            return _auditHits;
        }

        public void Reset()
        {
            TotalCorrupted = 0;
            CorruptedStations = 0;
            CorruptedTrunks = 0;
            CorruptedAnnouncements = 0;
            ManualFixes = 0;
            TotalChecked = 0;
            TotalToCheck = 0;
            _auditHits.Clear();
            Results.Clear();
        }

        public string GenerateReport()
        {
            var report = new StringBuilder();
            report.AppendLine("=== AUDIT STATISTICS REPORT ===");
            report.AppendLine();
            report.AppendLine($"CORRUPTED               : {TotalCorrupted}");
            report.AppendLine($"CORRUPTED STATIONS      : {CorruptedStations}");
            report.AppendLine($"CORRUPTED TRUNKS        : {CorruptedTrunks}");
            report.AppendLine($"CORRUPTED ANNOUNCEMENTS : {CorruptedAnnouncements}");
            report.AppendLine($"MANUAL FIXES            : {ManualFixes}");
            report.AppendLine();
            report.AppendLine("=== INDIVIDUAL AUDIT HITS ===");
            report.AppendLine();

            foreach (var kvp in _auditHits.OrderBy(x => x.Key))
            {
                if (kvp.Value > 0)
                    report.AppendLine($"{kvp.Key} : {kvp.Value}");
            }

            report.AppendLine();
            report.AppendLine($"RECORDS CHECKED : {TotalChecked}/{TotalToCheck}");
            return report.ToString();
        }

        public override string ToString()
        {
            return $"Corrupted: {TotalCorrupted} | Stations: {CorruptedStations} | Trunks: {CorruptedTrunks} | Announcements: {CorruptedAnnouncements} | Manual: {ManualFixes}";
        }
    }
}