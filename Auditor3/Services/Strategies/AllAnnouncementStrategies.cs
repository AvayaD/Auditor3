using Auditor3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditor3.Services.Strategies
{
    #region Announcement Strategies A01-A11

    public class AuditA01Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A01";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_INT_ANNC pr_int_annc) return CreateSuccess();
            if (pr_int_annc.AudioGroup == "00" && (!pr_int_annc.HasIANCBD() || !pr_int_annc.HasGMIANCBD()))
            {
                var fix = "";
                if (pr_int_annc.HasIANCBD() && !pr_int_annc.HasGMIANCBD())
                    fix = Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_int_annc.Board);
                else if (!pr_int_annc.HasIANCBD() && pr_int_annc.HasGMIANCBD())
                    fix = Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_int_annc.Board, pr_int_annc.IndexLName);
                else
                    fix = Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_int_annc.Board) + "\n" + Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_int_annc.Board, pr_int_annc.IndexLName);
                var message = FormatMessageWithFix("AUDIT-A01", "PR_INT_ANNC is missing board PREC", pr_int_annc.UID, fix);
                return CreateFailure(message, fix);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_INT_ANNC;
    }

    public class AuditA02Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A02";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_INT_ANNC pr_int_annc) return CreateSuccess();
            if (pr_int_annc.AudioGroup == "00") return CreateSuccess();

            var pr_ag_mbrs = Database.PR_AG_MBRs.FindAll(a => a.AudioGroup == pr_int_annc.AudioGroup);
            foreach (var pr_ag_mbr in pr_ag_mbrs)
            {
                var pr_ianc_bd = Database.PR_IANC_BDs.Find(a => a.UID == pr_int_annc.UID && a.Board == pr_ag_mbr.Board);
                var pr_gm_ianc_bd = Database.PR_GM_IANC_BDs.Find(a => a.UID == pr_int_annc.UID && a.Board == pr_ag_mbr.Board);
                if (pr_ianc_bd == null || pr_gm_ianc_bd == null)
                {
                    var fix = "";
                    if (pr_ianc_bd != null && pr_gm_ianc_bd == null)
                        fix = Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board);
                    else if (pr_ianc_bd == null && pr_gm_ianc_bd != null)
                        fix = Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board, pr_int_annc.IndexLName);
                    else
                        fix = Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board) + "\n" + Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board, pr_int_annc.IndexLName);
                    var message = FormatMessageWithFix("AUDIT-A02", "Missing audio group board PRECs", pr_int_annc.UID, fix);
                    return CreateFailure(message, fix);
                }
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_INT_ANNC;
    }

    public class AuditA03Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A03";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_IANC_BD pr_ianc_bd) return CreateSuccess();
            if (pr_ianc_bd.HasDuplicates())
            {
                var message = FormatMessage("AUDIT-A03", "Duplicate PR_IANC_BD", pr_ianc_bd.UID, $"Board: {pr_ianc_bd.Board}", "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_IANC_BD;
    }

    public class AuditA04Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A04";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_EXT pr_ext) return CreateSuccess();
            if (pr_ext.GID == "008c" && !pr_ext.HasINTANNC())
            {
                var message = FormatMessage("AUDIT-A04", "Missing PR_INT_ANNC", pr_ext.UID, $"EXT: {pr_ext.Digits}", "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_EXT;
    }

    public class AuditA05Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A05";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_INT_ANNC pr_int_annc) return CreateSuccess();
            if (!pr_int_annc.HasEXT())
            {
                var message = FormatMessage("AUDIT-A05", "PR_INT_ANNC is missing PR_EXT", pr_int_annc.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_INT_ANNC;
    }

    public class AuditA06Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A06";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_INT_ANNC pr_int_annc) return CreateSuccess();
            if (!pr_int_annc.HasUDATA())
            {
                var message = FormatMessage("AUDIT-A06", "PR_INT_ANNC is missing PR_UDATA", pr_int_annc.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_INT_ANNC;
    }

    public class AuditA07Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A07";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_UDATA pr_udata) return CreateSuccess();
            if (pr_udata.GID == "008c" && !pr_udata.HasINTANNC())
            {
                var message = FormatMessage("AUDIT-A07", "PR_UDATA is missing PR_INT_ANNC", pr_udata.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_UDATA;
    }

    public class AuditA08Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A08";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_INT_ANNC pr_int_annc) return CreateSuccess();
            if (pr_int_annc.HasAGMismatch())
            {
                Database.ManualUIDs.Add(pr_int_annc.UID);
                var message = FormatMessage("AUDIT-A08", "PR_INT_ANNC / PR_AN_GRP - AudioGroup Mismatch", pr_int_annc.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_INT_ANNC;
    }

    public class AuditA09Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A09";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_INT_ANNC pr_int_annc) return CreateSuccess();
            if (!pr_int_annc.HasANGRP())
            {
                var fixScript = Fixer.Add.PR_AN_GRP(pr_int_annc.UID, pr_int_annc.AudioGroup);
                var message = FormatMessageWithFix("AUDIT-A09", "Missing PR_AN_GRP", pr_int_annc.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_INT_ANNC;
    }

    public class AuditA10Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A10";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_GM_IANC_BD pr_gm_ianc_bd) return CreateSuccess();
            if (pr_gm_ianc_bd.HasDuplicates())
            {
                var fixScript = Fixer.Remove.PR_GM_IANC_BD(pr_gm_ianc_bd);
                var message = FormatMessageWithFix("AUDIT-A10", "Duplicate PR_GM_IANC_BD", pr_gm_ianc_bd.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_GM_IANC_BD;
    }

    public class AuditA11Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-A11";
        public override AuditCategory Category => AuditCategory.Announcement;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_GM_IANC_BD pr_gm_ianc_bd) return CreateSuccess();
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_GM_IANC_BD;
    }

    #endregion
}