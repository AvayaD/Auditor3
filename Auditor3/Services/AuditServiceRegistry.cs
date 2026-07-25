using Auditor3.Services.Strategies;
using System.Collections.Generic;
using System.Diagnostics;

namespace Auditor3.Services
{
    /// <summary>
    /// Service registry for dependency injection setup
    /// Creates and configures the AuditEngine with all strategies
    /// </summary>
    public static class AuditServiceRegistry
    {
        /// <summary>
        /// Creates and configures the AuditEngine with all 55 audit strategies
        /// </summary>
        public static AuditEngine CreateAuditEngine()
        {
            Debug.WriteLine("🏭 AuditServiceRegistry: Creating AuditEngine with all strategies...");

            var strategies = new List<IAuditStrategy>
            {
                // Station Strategies (S01-S35)
                new AuditS01Strategy(),
                new AuditS02Strategy(),
                new AuditS03Strategy(),
                new AuditS04Strategy(),
                new AuditS05Strategy(),
                new AuditS06Strategy(),
                new AuditS07Strategy(),
                new AuditS08Strategy(),
                new AuditS09Strategy(),
                new AuditS10Strategy(),
                new AuditS11Strategy(),
                new AuditS12Strategy(),
                new AuditS13Strategy(),
                new AuditS14Strategy(),
                new AuditS15Strategy(),
                new AuditS16Strategy(),
                new AuditS17Strategy(),
                new AuditS18Strategy(),
                new AuditS19Strategy(),
                new AuditS20Strategy(),
                new AuditS21Strategy(),
                new AuditS22Strategy(),
                new AuditS23Strategy(),
                new AuditS24Strategy(),
                new AuditS25Strategy(),
                new AuditS26Strategy(),
                new AuditS27Strategy(),
                new AuditS28Strategy(),
                new AuditS29Strategy(),
                new AuditS30Strategy(),
                new AuditS31Strategy(),
                new AuditS32Strategy(),
                new AuditS33Strategy(),
                new AuditS34Strategy(),
                new AuditS35Strategy(),

                // Announcement Strategies (A01-A11)
                new AuditA01Strategy(),
                new AuditA02Strategy(),
                new AuditA03Strategy(),
                new AuditA04Strategy(),
                new AuditA05Strategy(),
                new AuditA06Strategy(),
                new AuditA07Strategy(),
                new AuditA08Strategy(),
                new AuditA09Strategy(),
                new AuditA10Strategy(),
                new AuditA11Strategy(),

                // Trunk Strategies (T01-T09)
                new AuditT01Strategy(),
                new AuditT02Strategy(),
                new AuditT03Strategy(),
                new AuditT04Strategy(),
                new AuditT05Strategy(),
                new AuditT06Strategy(),
                new AuditT07Strategy(),
                new AuditT08Strategy(),
                new AuditT09Strategy(),
            };

            Debug.WriteLine($"✅ AuditServiceRegistry: Registered {strategies.Count} strategies");

            return new AuditEngine(strategies);
        }
    }
}