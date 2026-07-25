using Auditor3.Models;
using Auditor3.Services.Strategies;
using System.Collections.Generic;

namespace Auditor3.Services
{
    /// <summary>
    /// Static accessor for all audit strategies
    /// Provides direct access to strategies while maintaining original audit flow
    /// </summary>
    public static class AuditStrategies
    {
        private static readonly Dictionary<string, IAuditStrategy> _strategyCache =
            new Dictionary<string, IAuditStrategy>();

        static AuditStrategies()
        {
            // Initialize all strategies
            RegisterStrategy(new AuditS01Strategy());
            RegisterStrategy(new AuditS02Strategy());
            RegisterStrategy(new AuditS03Strategy());
            RegisterStrategy(new AuditS04Strategy());
            RegisterStrategy(new AuditS05Strategy());
            RegisterStrategy(new AuditS06Strategy());
            RegisterStrategy(new AuditS07Strategy());
            RegisterStrategy(new AuditS08Strategy());
            RegisterStrategy(new AuditS09Strategy());
            RegisterStrategy(new AuditS10Strategy());
            RegisterStrategy(new AuditS11Strategy());
            RegisterStrategy(new AuditS12Strategy());
            RegisterStrategy(new AuditS13Strategy());
            RegisterStrategy(new AuditS14Strategy());
            RegisterStrategy(new AuditS15Strategy());
            RegisterStrategy(new AuditS16Strategy());
            RegisterStrategy(new AuditS17Strategy());
            RegisterStrategy(new AuditS18Strategy());
            RegisterStrategy(new AuditS19Strategy());
            RegisterStrategy(new AuditS20Strategy());
            RegisterStrategy(new AuditS21Strategy());
            RegisterStrategy(new AuditS22Strategy());
            RegisterStrategy(new AuditS23Strategy());
            RegisterStrategy(new AuditS24Strategy());
            RegisterStrategy(new AuditS25Strategy());
            RegisterStrategy(new AuditS26Strategy());
            RegisterStrategy(new AuditS27Strategy());
            RegisterStrategy(new AuditS28Strategy());
            RegisterStrategy(new AuditS29Strategy());
            RegisterStrategy(new AuditS30Strategy());
            RegisterStrategy(new AuditS31Strategy());
            RegisterStrategy(new AuditS32Strategy());
            RegisterStrategy(new AuditS33Strategy());
            RegisterStrategy(new AuditS34Strategy());
            RegisterStrategy(new AuditS35Strategy());

            RegisterStrategy(new AuditA01Strategy());
            RegisterStrategy(new AuditA02Strategy());
            RegisterStrategy(new AuditA03Strategy());
            RegisterStrategy(new AuditA04Strategy());
            RegisterStrategy(new AuditA05Strategy());
            RegisterStrategy(new AuditA06Strategy());
            RegisterStrategy(new AuditA07Strategy());
            RegisterStrategy(new AuditA08Strategy());
            RegisterStrategy(new AuditA09Strategy());
            RegisterStrategy(new AuditA10Strategy());
            RegisterStrategy(new AuditA11Strategy());

            RegisterStrategy(new AuditT01Strategy());
            RegisterStrategy(new AuditT02Strategy());
            RegisterStrategy(new AuditT03Strategy());
            RegisterStrategy(new AuditT04Strategy());
            RegisterStrategy(new AuditT05Strategy());
            RegisterStrategy(new AuditT06Strategy());
            RegisterStrategy(new AuditT07Strategy());
            RegisterStrategy(new AuditT08Strategy());
            RegisterStrategy(new AuditT09Strategy());
        }

        private static void RegisterStrategy(IAuditStrategy strategy)
        {
            _strategyCache[strategy.Code] = strategy;
        }

        // Station Audits (S01-S35)
        internal static AuditResult AuditS01(PR_STN record) =>
            ExecuteStrategy<AuditS01Strategy>(record);

        internal static AuditResult AuditS02(PR_ST_CPS record) =>
            ExecuteStrategy<AuditS02Strategy>(record);

        internal static AuditResult AuditS03(PR_ST_CPS record) =>
            ExecuteStrategy<AuditS03Strategy>(record);

        internal static AuditResult AuditS04(PR_STN record) =>
            ExecuteStrategy<AuditS04Strategy>(record);

        internal static AuditResult AuditS05(PR_EXT record) =>
            ExecuteStrategy<AuditS05Strategy>(record);

        internal static AuditResult AuditS06(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS06Strategy>(record);

        internal static AuditResult AuditS07(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS07Strategy>(record);

        internal static AuditResult AuditS08(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS08Strategy>(record);

        internal static AuditResult AuditS09(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS09Strategy>(record);

        internal static AuditResult AuditS10(PR_STN record) =>
            ExecuteStrategy<AuditS10Strategy>(record);

        internal static AuditResult AuditS11(PR_BUTTON record) =>
            ExecuteStrategy<AuditS11Strategy>(record);

        internal static AuditResult AuditS12(PR_ST_CPS record) =>
            ExecuteStrategy<AuditS12Strategy>(record);

        internal static AuditResult AuditS13(PR_MOPORT record) =>
            ExecuteStrategy<AuditS13Strategy>(record);

        internal static AuditResult AuditS14(PR_MOPORT record) =>
            ExecuteStrategy<AuditS14Strategy>(record);

        internal static AuditResult AuditS15(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS15Strategy>(record);

        internal static AuditResult AuditS16(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS16Strategy>(record);

        internal static AuditResult AuditS17(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS17Strategy>(record);

        internal static AuditResult AuditS18(PR_BUTTON record) =>
            ExecuteStrategy<AuditS18Strategy>(record);

        internal static AuditResult AuditS19(PR_BRIDGE record) =>
            ExecuteStrategy<AuditS19Strategy>(record);

        internal static AuditResult AuditS20(PR_ST_CPS record) =>
            ExecuteStrategy<AuditS20Strategy>(record);

        internal static AuditResult AuditS21(PR_PORT_UID record) =>
            ExecuteStrategy<AuditS21Strategy>(record);

        internal static AuditResult AuditS22(PR_UDATA record) =>
            ExecuteStrategy<AuditS22Strategy>(record);

        internal static AuditResult AuditS23(PR_XMAP record) =>
            ExecuteStrategy<AuditS23Strategy>(record);

        internal static AuditResult AuditS24(PR_OPT_STN record) =>
            ExecuteStrategy<AuditS24Strategy>(record);

        internal static AuditResult AuditS25(PR_XMAP record) =>
            ExecuteStrategy<AuditS25Strategy>(record);

        internal static AuditResult AuditS26(PR_OPT_STN record) =>
            ExecuteStrategy<AuditS26Strategy>(record);

        internal static AuditResult AuditS27(PR_STN record) =>
            ExecuteStrategy<AuditS27Strategy>(record);

        internal static AuditResult AuditS28(PR_STN record) =>
            ExecuteStrategy<AuditS28Strategy>(record);

        internal static AuditResult AuditS29(PR_STN record) =>
            ExecuteStrategy<AuditS29Strategy>(record);

        internal static AuditResult AuditS30(PR_ST_CPS record) =>
            ExecuteStrategy<AuditS30Strategy>(record);

        internal static AuditResult AuditS31() =>
            new AuditResult("AUDIT-S31", AuditCategory.Station, true);

        internal static AuditResult AuditS32(PR_AMW record) =>
            ExecuteStrategy<AuditS32Strategy>(record);

        internal static AuditResult AuditS33(PR_AMW record) =>
            ExecuteStrategy<AuditS33Strategy>(record);

        internal static AuditResult AuditS34(PR_FEXT record) =>
            ExecuteStrategy<AuditS34Strategy>(record);

        internal static AuditResult AuditS35(PR_FEXT record) =>
            ExecuteStrategy<AuditS35Strategy>(record);

        // Announcement Audits (A01-A11)
        internal static AuditResult AuditA01(PR_INT_ANNC record) =>
            ExecuteStrategy<AuditA01Strategy>(record);

        internal static AuditResult AuditA02(PR_INT_ANNC record) =>
            ExecuteStrategy<AuditA02Strategy>(record);

        internal static AuditResult AuditA03(PR_IANC_BD record) =>
            ExecuteStrategy<AuditA03Strategy>(record);

        internal static AuditResult AuditA04(PR_EXT record) =>
            ExecuteStrategy<AuditA04Strategy>(record);

        internal static AuditResult AuditA05(PR_INT_ANNC record) =>
            ExecuteStrategy<AuditA05Strategy>(record);

        internal static AuditResult AuditA06(PR_INT_ANNC record) =>
            ExecuteStrategy<AuditA06Strategy>(record);

        internal static AuditResult AuditA07(PR_UDATA record) =>
            ExecuteStrategy<AuditA07Strategy>(record);

        internal static AuditResult AuditA08(PR_INT_ANNC record) =>
            ExecuteStrategy<AuditA08Strategy>(record);

        internal static AuditResult AuditA09(PR_INT_ANNC record) =>
            ExecuteStrategy<AuditA09Strategy>(record);

        internal static AuditResult AuditA10(PR_GM_IANC_BD record) =>
            ExecuteStrategy<AuditA10Strategy>(record);

        internal static AuditResult AuditA11(PR_GM_IANC_BD record, string audiogroup) =>
            ExecuteStrategy<AuditA11Strategy>(record);

        // Trunk Audits (T01-T09)
        internal static AuditResult AuditT01(PR_TR_MBR record) =>
            ExecuteStrategy<AuditT01Strategy>(record);

        internal static AuditResult AuditT02(PR_TRUNK record) =>
            ExecuteStrategy<AuditT02Strategy>(record);

        internal static AuditResult AuditT03(PR_TRUNK record) =>
            ExecuteStrategy<AuditT03Strategy>(record);

        internal static AuditResult AuditT04(PR_ACD_TRUNK record) =>
            ExecuteStrategy<AuditT04Strategy>(record);

        internal static AuditResult AuditT05(PR_TR_GRP record) =>
            ExecuteStrategy<AuditT05Strategy>(record);

        internal static AuditResult AuditT06(bool measured, PR_TR_MBR record) =>
            ExecuteStrategy<AuditT06Strategy>(record);

        internal static AuditResult AuditT07(PR_MOPORT record) =>
            ExecuteStrategy<AuditT07Strategy>(record);

        internal static AuditResult AuditT08(PR_PORT_UID record) =>
            ExecuteStrategy<AuditT08Strategy>(record);

        internal static AuditResult AuditT09(PR_MOPORT record) =>
            ExecuteStrategy<AuditT09Strategy>(record);

        // Helper method to execute strategy
        private static AuditResult ExecuteStrategy<T>(object record) where T : IAuditStrategy, new()
        {
            var strategy = new T();
            return strategy.Audit(record);
        }
    }
}