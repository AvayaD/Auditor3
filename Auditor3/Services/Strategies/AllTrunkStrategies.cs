using Auditor3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditor3.Services.Strategies
{
    #region Trunk Strategies T01-T09

    public class AuditT01Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T01";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_TR_MBR pr_tr_mbr) return CreateSuccess();
            if (!pr_tr_mbr.HasTRUNK())
            {
                var message = FormatMessage("AUDIT-T01", "PR_TR_MBR is missing PR_TRUNK", pr_tr_mbr.UID, "TrunkGroup: " + pr_tr_mbr.TrunkGroup, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_TR_MBR;
    }

    public class AuditT02Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T02";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_TRUNK pr_trunk) return CreateSuccess();
            if (!pr_trunk.HasTRMBR())
            {
                var message = FormatMessage("AUDIT-T02", "PR_TRUNK is missing PR_TR_MBR", pr_trunk.UID, "TrunkGroup: " + pr_trunk.TrunkGroup, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_TRUNK;
    }

    public class AuditT03Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T03";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_TRUNK pr_trunk) return CreateSuccess();
            if (!pr_trunk.HasMOPORT() || !pr_trunk.HasPORTUID())
            {
                var fixScript = Fixer.Add.TrunkPort(pr_trunk);
                var message = FormatMessageWithFix("AUDIT-T03", "PR_TRUNK is missing port PRECs", pr_trunk.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_TRUNK;
    }

    public class AuditT04Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T04";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_ACD_TRUNK pr_acd_trunk) return CreateSuccess();
            if (pr_acd_trunk.HasDuplicates())
            {
                var pr_acd_trunks = Database.PR_ACD_TRUNKs.FindAll(a =>
                    a.TrunkGroupUID == pr_acd_trunk.TrunkGroupUID &&
                    a.TrunkMemberUID == pr_acd_trunk.TrunkMemberUID);
                foreach (var trunk in pr_acd_trunks) trunk.Flagged = true;

                var fixes = string.Join("\n", Enumerable.Range(1, pr_acd_trunks.Count - 1)
                    .Select(_ => Fixer.Remove.PR_ACD_TRUNK(pr_acd_trunk.TrunkGroupUID, pr_acd_trunk.TrunkMemberUID)));

                var message = FormatMessageWithFix("AUDIT-T04", "Duplicate PR_ACD_TRUNK record", pr_acd_trunk.TrunkGroupUID, fixes);
                return CreateFailure(message, fixes);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_ACD_TRUNK;
    }

    public class AuditT05Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T05";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_TR_GRP pr_tr_grp) return CreateSuccess();
            if (!pr_tr_grp.Measured && pr_tr_grp.HasACDTRUNK())
            {
                var pr_acd_trunks = Database.PR_ACD_TRUNKs.FindAll(a => a.TrunkGroupUID == pr_tr_grp.UID);
                var fixes = string.Join("\n", pr_acd_trunks.Select(t =>
                    Fixer.Remove.PR_ACD_TRUNK(t.TrunkGroupUID, t.TrunkMemberUID)));

                var message = FormatMessageWithFix("AUDIT-T05", "PR_ACD_TRUNK exists on unmeasured trunk group", pr_tr_grp.UID, fixes);
                return CreateFailure(message, fixes);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_TR_GRP;
    }

    public class AuditT06Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T06";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_TR_MBR pr_tr_mbr) return CreateSuccess();
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_TR_MBR;
    }

    public class AuditT07Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T07";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_MOPORT pr_moport) return CreateSuccess();
            if (!pr_moport.HasPORTUID())
            {
                string fix;
                if (pr_moport.HasTRUNK())
                {
                    var pr_trunk = Database.PR_TRUNKs.Find(a => a.Port == pr_moport.Port);
                    fix = Fixer.Add.TrunkPort(pr_trunk);
                }
                else
                    fix = Fixer.Remove.PR_MOPORT(pr_moport.Port);

                var message = FormatMessageWithFix("AUDIT-T07", "PR_MOPORT is missing PR_PORT_UID", "0", fix);
                return CreateFailure(message, fix);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_MOPORT;
    }

    public class AuditT08Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T08";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (!pr_port_uid.HasMOPORT())
            {
                string fix;
                if (pr_port_uid.HasTRUNK())
                {
                    var pr_trunk = Database.PR_TRUNKs.Find(a => a.Port == pr_port_uid.Port);
                    fix = Fixer.Add.TrunkPort(pr_trunk);
                }
                else
                    fix = Fixer.Remove.PR_PORT_UID(pr_port_uid.Port);

                var message = FormatMessageWithFix("AUDIT-T08", "PR_PORT_UID is missing PR_MOPORT", pr_port_uid.UID, fix);
                return CreateFailure(message, fix);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditT09Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-T09";
        public override AuditCategory Category => AuditCategory.Trunk;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_MOPORT pr_moport) return CreateSuccess();
            if (!pr_moport.HasTRUNK() && pr_moport.TGUID().Substring(0, 4) == "0005")
            {
                var fix = Fixer.Remove.PR_MOPORT(pr_moport.Port);
                if (pr_moport.HasPORTUID())
                    fix += "\n" + Fixer.Remove.PR_PORT_UID(pr_moport.Port);

                var message = FormatMessageWithFix("AUDIT-T09", "PR_MOPORT is missing PR_TRUNK", pr_moport.TGUID(), fix);
                return CreateFailure(message, fix);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_MOPORT;
    }

    #endregion
}