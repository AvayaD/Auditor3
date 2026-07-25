using Auditor3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditor3.Services.Strategies
{
    #region Station Strategies S01-S35

    public class AuditS01Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S01";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_STN pr_stn) return CreateSuccess();
            if (!pr_stn.HasUDATA())
            {
                var fixScript = Fixer.Remove.Station(pr_stn.UID);
                var message = FormatMessageWithFix("AUDIT-S01", "PR_STN is missing PR_UDATA", pr_stn.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_STN;
    }

    public class AuditS02Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S02";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_ST_CPS pr_st_cps) return CreateSuccess();
            if (!pr_st_cps.HasPORTUID() && !pr_st_cps.HasMOPORT() && pr_st_cps.HasSTN() && !pr_st_cps.HasDuplicates())
            {
                var fixScript = Fixer.Add.PR_MOPORT(pr_st_cps.UID, pr_st_cps.Port) + "\n" + Fixer.Add.PR_PORT_UID(pr_st_cps.UID, pr_st_cps.Port);
                var message = FormatMessageWithFix("AUDIT-S02", "PR_ST_CPS is missing PR_PORT_UID and PR_MOPORT", pr_st_cps.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_ST_CPS;
    }

    public class AuditS03Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S03";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_ST_CPS pr_st_cps) return CreateSuccess();
            if (!pr_st_cps.HasPORTUID() && !pr_st_cps.HasMOPORT() && pr_st_cps.HasSTN() && pr_st_cps.HasDuplicates())
            {
                var pr_stn = Database.PR_STNs.Find(a => a.UID == pr_st_cps.UID);
                var fixScript = pr_stn.IsIP() ? Fixer.Add.NewIPPort(pr_st_cps.UID) : "** UNABLE TO ASSIGN TDM PORT AUTOMATICALLY **\n** MANUAL FIX REQUIRED **";
                var message = FormatMessageWithFix("AUDIT-S03", "Incorrect port assigned in PR_ST_CPS", pr_st_cps.UID, fixScript);
                return CreateFailure(message, fixScript, requiresManualFix: !pr_stn.IsIP());
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_ST_CPS;
    }

    public class AuditS04Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S04";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_STN pr_stn) return CreateSuccess();
            if (pr_stn.GID != "0002" && !pr_stn.HasEXT())
            {
                var fixScript = Fixer.Remove.Station(pr_stn.UID);
                var message = FormatMessageWithFix("AUDIT-S04", "PR_STN is missing PR_EXT", pr_stn.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_STN;
    }

    public class AuditS05Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S05";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_EXT pr_ext) return CreateSuccess();
            if (pr_ext.GID == "0000" && !pr_ext.HasUDATA())
            {
                var fixScript = Fixer.Remove.PR_EXT(pr_ext.Digits);
                var message = FormatMessageWithFix("AUDIT-S05", "PR_EXT has no PR_UDATA", pr_ext.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_EXT;
    }

    public class AuditS06Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S06";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (!pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS())
            {
                var fixScript = Fixer.Remove.PR_PORT_UID(pr_port_uid.Port);
                var message = FormatMessageWithFix("AUDIT-S06", "PR_PORT_UID has neither PR_MOPORT nor PR_ST_CPS", pr_port_uid.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS07Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S07";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (!pr_port_uid.HasMOPORT() && pr_port_uid.HasSTCPS() && !pr_port_uid.HasDuplicateSTCPS())
            {
                var fixScript = Fixer.Add.PR_MOPORT(pr_port_uid.UID, pr_port_uid.Port);
                var message = FormatMessageWithFix("AUDIT-S07", "PR_PORT_UID is missing PR_MOPORT", pr_port_uid.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS08Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S08";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS() && pr_port_uid.UID != Globals.NULL_UID && pr_port_uid.GID != "0034" && !pr_port_uid.UIDHasDuplicatePort() && !pr_port_uid.UIDOwnsAnotherPort() && pr_port_uid.HasSTN())
            {
                var fixScript = Fixer.Add.PR_ST_CPS(pr_port_uid.UID, pr_port_uid.Port);
                var message = FormatMessageWithFix("AUDIT-S08", "PR_PORT_UID is missing PR_ST_CPS", pr_port_uid.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS09Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S09";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            var ports = Database.PR_ST_CPSs.FindAll(a => a.Port == pr_port_uid.Port);
            var issues = ports.Where(a => a.UID != pr_port_uid.UID && a.HasSTN()).ToList();
            if (issues.Count > 0)
            {
                var fixes = string.Join("\n", issues.Select(p => Fixer.Update.PR_ST_CPS(p.UID)));
                var message = FormatMessageWithFix("AUDIT-S09", "Incorrect PR_ST_CPS - Duplicate port issue", pr_port_uid.Port, fixes);
                return CreateFailure(message, fixes);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS10Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S10";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_STN pr_stn) return CreateSuccess();
            if (pr_stn.IsIP() && !pr_stn.HasValidIPPort())
            {
                var fixScript = Fixer.Add.NewIPPort(pr_stn.UID);
                var message = FormatMessageWithFix("AUDIT-S10", "IP station does not have valid 7fxxxxxx port", pr_stn.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_STN;
    }

    public class AuditS11Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S11";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_BUTTON pr_button) return CreateSuccess();
            if (pr_button.Number == "0001" && !pr_button.HasSTN() && !pr_button.HasUDATA())
            {
                var fixScript = Fixer.Remove.PR_BUTTON(pr_button.UID, "0001");
                var message = FormatMessageWithFix("AUDIT-S11", "PR_BUTTON has neither PR_STN nor PR_UDATA", pr_button.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_BUTTON;
    }

    public class AuditS12Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S12";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_ST_CPS pr_st_cps) return CreateSuccess();
            if (!pr_st_cps.HasPORTUID() && pr_st_cps.HasMOPORT() && !pr_st_cps.HasDuplicates())
            {
                var fixScript = Fixer.Add.PR_PORT_UID(pr_st_cps.UID, pr_st_cps.Port);
                var message = FormatMessageWithFix("AUDIT-S12", "PR_ST_CPS is missing PR_PORT_UID", pr_st_cps.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_ST_CPS;
    }

    public class AuditS13Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S13";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_MOPORT pr_moport) return CreateSuccess();
            if (pr_moport.HasSTCPS() && !pr_moport.HasPORTUID())
            {
                var fixScript = Fixer.Add.PR_PORT_UID(pr_moport.UID(), pr_moport.Port);
                var message = FormatMessageWithFix("AUDIT-S13", "PR_MOPORT is missing PR_PORT_UID", pr_moport.UID(), fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_MOPORT;
    }

    public class AuditS14Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S14";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_MOPORT pr_moport) return CreateSuccess();
            if (!pr_moport.HasSTCPS() && !pr_moport.HasPORTUID())
            {
                var fixScript = Fixer.Remove.PR_MOPORT(pr_moport.Port);
                var message = FormatMessageWithFix("AUDIT-S14", "PR_MOPORT is abandoned", pr_moport.UID(), fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_MOPORT;
    }

    public class AuditS15Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S15";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (pr_port_uid.HasMOPORT() && pr_port_uid.UID == Globals.NULL_UID && pr_port_uid.ValidOwner() != null)
            {
                var fixScript = Fixer.Update.PR_PORT_UID(pr_port_uid.Port, pr_port_uid.ValidOwner());
                var message = FormatMessage("AUDIT-S15", "PR_PORT_UID has incorrect UID", $"Old UID: {pr_port_uid.UID}", $"New UID: {pr_port_uid.ValidOwner()}", $"Fix: {fixScript}");
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS16Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S16";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (pr_port_uid.HasMOPORT() && pr_port_uid.UID == Globals.NULL_UID && pr_port_uid.ValidOwner() == null)
            {
                var fixScript = Fixer.Remove.PR_PORT_UID(pr_port_uid.Port) + "\n" + Fixer.Remove.PR_MOPORT(pr_port_uid.Port);
                var message = FormatMessageWithFix("AUDIT-S16", "PR_PORT_UID and PR_MOPORT are abandoned", pr_port_uid.Port, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS17Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S17";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS() && pr_port_uid.UID != Globals.NULL_UID && pr_port_uid.GID != "0034" && !pr_port_uid.UIDOwnsAnotherPort() && !pr_port_uid.UIDHasDuplicatePort())
            {
                var fixScript = Fixer.Remove.PR_MOPORT(pr_port_uid.Port) + "\n" + Fixer.Remove.PR_PORT_UID(pr_port_uid.Port);
                var message = FormatMessageWithFix("AUDIT-S17", "PR_PORT_UID and PR_MOPORT are abandoned", pr_port_uid.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS18Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S18";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_BUTTON pr_button) return CreateSuccess();
            if (pr_button.Bridged && !pr_button.HasValidBridgedUID())
            {
                var fixScript = Fixer.Remove.PR_BUTTON(pr_button.UID, pr_button.Number);
                var message = FormatMessageWithFix("AUDIT-S18", "Bridged PR_BUTTON has invalid target UID", pr_button.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_BUTTON;
    }

    public class AuditS19Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S19";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_BRIDGE pr_bridge) return CreateSuccess();
            if (!pr_bridge.HasValidPrimaryUID())
            {
                var fixScript = Fixer.Remove.PR_BRIDGE(pr_bridge);
                var message = FormatMessageWithFix("AUDIT-S19", "PR_BRIDGE has invalid primary UID", pr_bridge.PrimaryUID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_BRIDGE;
    }

    public class AuditS20Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S20";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_ST_CPS pr_st_cps) return CreateSuccess();
            if (!pr_st_cps.HasSTN() && !pr_st_cps.HasUDATA())
            {
                var fixScript = Fixer.Remove.PR_ST_CPS(pr_st_cps.UID);
                var message = FormatMessageWithFix("AUDIT-S20", "PR_ST_CPS is missing PR_STN and PR_UDATA", pr_st_cps.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_ST_CPS;
    }

    public class AuditS21Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S21";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_PORT_UID pr_port_uid) return CreateSuccess();
            if (pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS() && pr_port_uid.UID != Globals.NULL_UID && pr_port_uid.GID != "0034" && !pr_port_uid.UIDHasDuplicatePort() && !pr_port_uid.UIDOwnsAnotherPort() && !pr_port_uid.HasSTN())
            {
                var fixScript = Fixer.Remove.PR_PORT_UID(pr_port_uid.Port) + "\n" + Fixer.Remove.PR_MOPORT(pr_port_uid.Port);
                var message = FormatMessageWithFix("AUDIT-S21", "Port is abandoned", pr_port_uid.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_PORT_UID;
    }

    public class AuditS22Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S22";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_UDATA pr_udata) return CreateSuccess();
            if (!pr_udata.HasSTN())
            {
                var fixScript = Fixer.Remove.Station(pr_udata.UID);
                var message = FormatMessageWithFix("AUDIT-S22", "PR_UDATA does not have PR_STN", pr_udata.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_UDATA;
    }

    public class AuditS23Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S23";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_XMAP pr_xmap) return CreateSuccess();
            if (!pr_xmap.HasOPTSTN() && !pr_xmap.IsXMOBILE())
            {
                var fixScript = Fixer.Remove.PR_XMAP(pr_xmap);
                var message = FormatMessageWithFix("AUDIT-S23", "PR_XMAP does not have PR_OPT_STN", pr_xmap.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_XMAP;
    }

    public class AuditS24Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S24";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_OPT_STN pr_opt_stn) return CreateSuccess();
            if (!pr_opt_stn.HasXMAP())
            {
                var fixScript = Fixer.Remove.PR_OPT_STN(pr_opt_stn);
                var message = FormatMessageWithFix("AUDIT-S24", "PR_OPT_STN does not have PR_XMAP", pr_opt_stn.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_OPT_STN;
    }

    public class AuditS25Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S25";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_XMAP pr_xmap) return CreateSuccess();
            if (!pr_xmap.HasUDATA())
            {
                Database.ManualUIDs.Add(pr_xmap.UID);
                var message = FormatMessage("AUDIT-S25", "PR_XMAP does not have PR_UDATA", pr_xmap.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, string.Join(Environment.NewLine, pr_xmap.PREC), true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_XMAP;
    }

    public class AuditS26Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S26";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_OPT_STN pr_opt_stn) return CreateSuccess();
            if (!pr_opt_stn.HasUDATA())
            {
                Database.ManualUIDs.Add(pr_opt_stn.UID);
                var message = FormatMessage("AUDIT-S26", "PR_OPT_STN does not have PR_UDATA", pr_opt_stn.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, string.Join(Environment.NewLine, pr_opt_stn.PREC), true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_OPT_STN;
    }

    public class AuditS27Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S27";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_STN pr_stn) return CreateSuccess();
            if (!pr_stn.HasFEXT() && pr_stn.HasEXT())
            {
                var fixScript = Fixer.Add.PR_FEXT(pr_stn.UID);
                var message = FormatMessageWithFix("AUDIT-S27", "PR_STN does not have PR_FEXT", pr_stn.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_STN;
    }

    public class AuditS28Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S28";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_STN pr_stn) return CreateSuccess();
            if (pr_stn.HasEXT() && pr_stn.HasUDATA() && !pr_stn.HasMatchingDigits())
            {
                var fixScript = Fixer.Update.PR_FEXT(pr_stn.UID);
                var message = FormatMessageWithFix("AUDIT-S28", "PR_STN has mismatched digits in PR_EXT and PR_FEXT", pr_stn.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_STN;
    }

    public class AuditS29Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S29";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_STN pr_stn) return CreateSuccess();
            if (pr_stn.HasAWOHMismatch())
            {
                if (pr_stn.IsIP() && !pr_stn.AWOH) return CreateSuccess();
                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.UID == pr_stn.UID);
                var port = pr_st_cps != null ? pr_st_cps.Port : Globals.NULL_PORT;

                if (pr_stn.IsIP() && pr_stn.AWOH)
                {
                    Database.ManualUIDs.Add(pr_stn.UID);
                    if (port != Globals.NULL_PORT) Database.ManualPorts.Add(port);
                    var message = FormatMessage("AUDIT-S29", "AWOH MISMATCH", pr_stn.UID, $"PORT: {port}", "** IP STATION CANNOT BE AWOH **", "** MANUAL FIX REQUIRED **");
                    return CreateFailure(message, "", true);
                }
                else if (!pr_stn.IsIP() && pr_stn.AWOH && pr_stn.HasValidIPPort())
                {
                    var fixScript = Fixer.Remove.PR_MOPORT(port) + "\n" + Fixer.Remove.PR_PORT_UID(port) + "\n" + Fixer.Update.PR_ST_CPS(pr_stn.UID, Globals.NULL_PORT);
                    var message = FormatMessageWithFix("AUDIT-S29", "AWOH TDM station has IP port", pr_stn.UID, fixScript);
                    return CreateFailure(message, fixScript);
                }
                else if (!pr_stn.IsIP() && pr_stn.AWOH && port != Globals.NULL_PORT)
                {
                    Database.ManualUIDs.Add(pr_stn.UID);
                    if (port != Globals.NULL_PORT) Database.ManualPorts.Add(port);
                    var message = FormatMessage("AUDIT-S29", "AWOH MISMATCH", pr_stn.UID, $"PORT: {port}", "** TDM STATION WITH PORT **", "** MANUAL FIX REQUIRED **");
                    return CreateFailure(message, "", true);
                }
                else if (!pr_stn.IsIP() && !pr_stn.AWOH && port == Globals.NULL_PORT)
                {
                    Database.ManualUIDs.Add(pr_stn.UID);
                    var message = FormatMessage("AUDIT-S29", "AWOH MISMATCH", pr_stn.UID, "** TDM STATION WITHOUT PORT **", "** MANUAL FIX REQUIRED **");
                    return CreateFailure(message, "", true);
                }
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_STN;
    }

    public class AuditS30Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S30";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_ST_CPS pr_st_cps) return CreateSuccess();
            if (!pr_st_cps.IsIPPort() && pr_st_cps.Port != Globals.NULL_PORT && !pr_st_cps.HasMOBD())
            {
                Database.ManualUIDs.Add(pr_st_cps.UID);
                var message = FormatMessage("AUDIT-S30", "Missing PR_MOBD", pr_st_cps.UID, $"Board: {pr_st_cps.Port}", "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_ST_CPS;
    }

    public class AuditS31Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S31";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => false;
    }

    public class AuditS32Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S32";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_AMW pr_amw) return CreateSuccess();
            if (!pr_amw.DupFlagged && pr_amw.HasDuplicates())
            {
                var pr_amws = Database.PR_AMWs.FindAll(a => a.ActualUID == pr_amw.ActualUID && a.Extension == pr_amw.Extension);
                var removeCount = pr_amws.Count - 1;
                var fixes = string.Join("\n", Enumerable.Range(0, removeCount).Select(_ => Fixer.Remove.PR_AMW(pr_amw)));
                var message = FormatMessageWithFix("AUDIT-S32", "PR_AMW is duplicate", pr_amw.ActualUID, fixes);
                return CreateFailure(message, fixes);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_AMW;
    }

    public class AuditS33Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S33";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_AMW pr_amw) return CreateSuccess();
            if (pr_amw.IsMismatched() || pr_amw.MwlExtMismatch())
            {
                var fix = "";
                if (pr_amw.IsMismatched())
                    fix += Fixer.Update.PR_AMW(pr_amw) + "\n";
                if (pr_amw.MwlExtMismatch())
                    fix += Fixer.Update.PR_AMW_REMOVE(pr_amw);
                var message = FormatMessageWithFix("AUDIT-S33", "PR_AMW is mismatched", pr_amw.ActualUID, fix);
                return CreateFailure(message, fix);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_AMW;
    }

    public class AuditS34Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S34";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_FEXT pr_fext) return CreateSuccess();
            if (pr_fext.IsStation && !pr_fext.HasUDATA())
            {
                var fixScript = Fixer.Remove.PR_FEXT(pr_fext);
                var message = FormatMessageWithFix("AUDIT-S34", "PR_FEXT is orphaned", pr_fext.UID, fixScript);
                return CreateFailure(message, fixScript);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_FEXT;
    }

    public class AuditS35Strategy : BaseAuditStrategy
    {
        public override string Code => "AUDIT-S35";
        public override AuditCategory Category => AuditCategory.Station;
        public override AuditResult Audit(object record)
        {
            if (record is not PR_FEXT pr_fext) return CreateSuccess();
            if (pr_fext.HasDuplicateUID())
            {
                Database.ManualUIDs.Add(pr_fext.UID);
                var message = FormatMessage("AUDIT-S35", "PR_FEXT is duplicate", pr_fext.UID, "** MANUAL FIX REQUIRED **");
                return CreateFailure(message, "", true);
            }
            return CreateSuccess();
        }
        public override bool CanHandle(object record) => record is PR_FEXT;
    }

    #endregion
}