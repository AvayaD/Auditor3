/*
 * Auditor3 :: Fixer
 * 
 * This class defines the process that generates the fixes for issues that are found.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Auditor3 {
    internal static class Fixer {

        internal static List<string> FixLines;          // List to store the lines for the fix script

        // This method is used to initialize the fix script storage list
        internal static void Initialize() {
            FixLines = new List<string>();
        }

        // This method is for generating the fixscript
        internal static void GenerateFixscript() {
            if (FixLines.Count == 0) return;
            var fixscript = $"{Globals.REPORT_DIR}fixscript_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
            var writer = new StreamWriter(fixscript);
            writer.Write(string.Join(Environment.NewLine, FixLines));
            writer.Close();
            Globals.GUI.AddOutput($"Fixscript generated at {fixscript}");
            Globals.GUI.AddOutput("");
        }

        // This method adds fix lines to the script list
        internal static void AddFix(string fix) {
            // Split on any newlines and remove any empty lines
            var fixes = fix.Split('\n').Where(a => !string.IsNullOrEmpty(a));

            // Add the lines to the list
            foreach (var line in fixes) {
                // Make sure this line is a 'prec ...' command
                if (!line.StartsWith("prec")) continue;
                FixLines.Add(line);
            }
        }

        // Class of methods used for generating add commands
        internal static class Add {

            // This method is for adding a PR_MOPORT record
            internal static string PR_MOPORT(string uid, string port) {
                // Try and get the STN object
                var pr_stn = Database.PR_STNs.Find(a => a.UID == uid);

                // Define a variable to hold the fix
                var fix = "";

                // Check if we have the MO value
                if (pr_stn?.GetMO() != null) {
                    var pr_ttiset = Database.PR_TTISETs.Find(a => a.UID == uid);
                    var tti = pr_ttiset == null ? "0" : "1";
                    var mo = port.StartsWith("7f") ? "0444" : pr_stn.GetMO();
                    fix = $"prec pr_moport a l0x{tti}{mo} l0x0 l0x{port} l0x0";
                    AddFix(fix);
                } else {
                    fix = $"** UNABLE TO DETERMINE MO VALUE **{Environment.NewLine}** MANUAL FIX REQUIRED **";
                    Audits.ManualFixes++;
                    Database.ManualPorts.Add(port);
                    Database.ManualUIDs.Add(uid);
                }

                // Return the fix for the report
                return fix;
            }

            // This method is for adding a PR_PORT_UID record
            internal static string PR_PORT_UID(string uid, string port) {
                // Create the fix line, add it to the fix list and return it for the report
                var fix = $"prec pr_port_uid a l0x{port} l0x{uid}";
                AddFix(fix);
                return fix;
            }

            // This method is for adding a PR_ST_CPS record
            internal static string PR_ST_CPS(string uid, string port) {
                // Create the fix
                var fix = $"prec pr_st_cps l l0x{uid}{Environment.NewLine}prec pr_st_cps a l0x{uid} l0x{port} l0x0 l0x0 l0x0";
                AddFix(fix);
                return fix;
            }

            // This method is for adding a PR_FEXT record
            internal static string PR_FEXT(string uid) {
                // Get the PR_EXT record, create the fix, add it to the list, and return it
                var pr_ext = Database.PR_EXTs.Find(a => a.UID == uid);
                var split = pr_ext.PREC[0].Split(' ');
                var fix = $"prec pr_fext a l0x{split[1]} l0x{split[2]} l0x{split[3]} l0x{split[4]}";
                AddFix(fix);
                return fix;
            }

            // This method is for adding a PR_IANC_BD record
            internal static string PR_IANC_BD(string uid, string board, string indexlname) {
                // Create the fix, add it to the list, and return it
                var fix = $"prec pr_ianc_bd a l0x{uid} l0x{board} l0x{indexlname} l0x4000";
                AddFix(fix);
                return fix;
            }

            // This method is for adding a PR_GM_IANC_BD record
            internal static string PR_GM_IANC_BD(string uid, string board) {
                // Create the fix, add it to the list, and return it
                var fix = $"prec pr_gm_ianc_bd a l0x{uid} l0x{board} l0x1";
                AddFix(fix);
                return fix;
            }

            // This method is for adding a PR_ACD_TRUNK record
            internal static string PR_ACD_TRUNK(PR_TR_MBR pr_tr_mbr) {
                // Get any PR_ACD_TRUNK from the group
                var pr_acd_trunk = Database.PR_ACD_TRUNKs.Find(a => a.TrunkGroupUID == pr_tr_mbr.TrunkGroup);
                if (pr_acd_trunk == null) {
                    return null;
                } else {
                    var fix = $"prec pr_acd_trunk a l0x{pr_tr_mbr.TrunkGroup} l0x{pr_tr_mbr.UID} l0x{pr_acd_trunk.TrunkType} l0x{pr_acd_trunk.Split} l0x{pr_tr_mbr.Port} l0x{pr_acd_trunk.MeasuredBy}";
                    AddFix(fix);
                    return fix;
                }
            }

            // This method is for adding a PR_AN_GRP record
            internal static string PR_AN_GRP(string uid, string audiogroup) {
                var fix = $"prec pr_an_grp a l0x{uid} l0x0 l0x1 l0x{audiogroup}02";
                AddFix(fix);
                return fix;
            }

            // This method is for adding a new IP port
            internal static string NewIPPort(string uid) {
                
                // Define a stringbuilder for storing the commands
                var fix = new StringBuilder();

                // Get the PR_STN record and make sure we have a MO value
                var pr_stn = Database.PR_STNs.Find(a => a.UID == uid);
                if (pr_stn?.GetMO() != null) {
                    // Lets get a free port
                    var port = Database.FindUnusedIPPort();
                    if (port != null) {
                        // We have a port and a MO, now get the PR_ST_CPS record and see if we need
                        // to add or update
                        var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.UID == uid);
                        if (pr_st_cps != null) {
                            fix.AppendLine($"prec pr_st_cps l l0x{uid}");
                            fix.AppendLine($"prec pr_st_cps w l0x{uid} l0x{port} l0x0 l0x0 l0x0");
                        } else {
                            fix.AppendLine($"prec pr_st_cps a l0x{uid} l0x{port} l0x0 l0x0 l0x0");
                        }

                        fix.AppendLine($"prec pr_port_uid a l0x{port} l0x{uid}");
                        fix.AppendLine($"prec pr_moport a l0x{pr_stn.GetMO()} l0x0 l0x{port} l0x0");

                        AddFix(fix.ToString());
                    } else {
                        // We didn't get a port for some reason
                        fix.AppendLine("** UNABLE TO DETERMINE NEW IP PORT TO USE **");
                        fix.AppendLine("** MANUAL FIX REQUIRED **");
                        Audits.ManualFixes++;
                        Database.ManualUIDs.Add(uid);
                    }
                } else {
                    // Not able to get the correct MO
                    fix.AppendLine("** UNABLE TO DETERMINE CORRECT MO VALUE **");
                    fix.AppendLine("** MANUAL FIX REQUIRED **");
                    Audits.ManualFixes++;
                    Database.ManualUIDs.Add(uid);
                }

                // Return the fixes for the report
                return fix.ToString();
            }

            // This method is used to add missing trunk port PRECs
            internal static string TrunkPort(PR_TRUNK pr_trunk) {
                // Create the fix object
                var fix = new StringBuilder();

                if (!pr_trunk.HasMOPORT()) {
                    var pr_trunks = Database.PR_TRUNKs.FindAll(a => a.TrunkGroup == pr_trunk.TrunkGroup);
                    var moFound = false;
                    var mo = "";

                    foreach (var check in pr_trunks) {
                        var pr_moport = Database.PR_MOPORTs.Find(a => a.Port == check.Port);
                        if (pr_moport != null) {
                            moFound = true;
                            mo = pr_moport.MO;
                            break;
                        }
                    }

                    if (!moFound) {
                        fix.AppendLine("** NO MO FOUND FOR THIS TRUNK GROUP **");
                        fix.AppendLine("** MANUAL FIX REQUIRED **");
                        Audits.ManualFixes++;
                        return fix.ToString();
                    }

                    fix.AppendLine($"prec pr_moport a l0x{mo} l0x0 l0x{pr_trunk.Port} l0x0");
                }

                if (!pr_trunk.HasPORTUID()) {
                    fix.AppendLine($"prec pr_port_uid a l0x{pr_trunk.Port} l0x{pr_trunk.UID}");
                }

                AddFix(fix.ToString());
                return fix.ToString();
            }
        }

        // Class of methods used for generating remove commands
        internal static class Remove {
            // Method for removing a PR_AMW record
            internal static string PR_AMW(PR_AMW pr_amw) {
                var fields = pr_amw.PREC[0].Split(' ');
                var fix = $"prec pr_amw d l0x{fields[1]} l0x{fields[2]} l0x{fields[3]}";
                AddFix(fix);
                return fix;
            }

            // Method for removing a PR_GM_IANC_BD record
            internal static string PR_GM_IANC_BD(PR_GM_IANC_BD pr_gm_ianc_bd) {
                var fix = $"prec pr_gm_ianc_bd d l0x{pr_gm_ianc_bd.UID} l0x{pr_gm_ianc_bd.Board}";
                AddFix(fix);
                return fix;
            }

            // This method is for removing a PR_ACD_TRUNK record
            internal static string PR_ACD_TRUNK(string trunkgroupuid, string trunkmemberuid) {
                var fix = $"prec pr_acd_trunk d l0x{trunkgroupuid} l0x{trunkmemberuid}";
                AddFix(fix);
                return fix;
            }

            // This method is for removing a station's records
            internal static string Station(string uid) {
                bool extFound = false;
                bool portFound = false;
                var ext1 = "";
                var ext2 = "";
                var port = "";

                var pr_ext = Database.PR_EXTs.Find(a => a.UID == uid);
                if (pr_ext != null) {
                    var pr_ext_split = pr_ext.PREC[0].Split(' ');
                    ext1 = pr_ext_split[1];
                    ext2 = pr_ext_split[2];
                    extFound = true;
                }

                if (!extFound) {
                    var pr_fext = Database.PR_FEXTs.Find(a => a.UID == uid);
                    if (pr_fext != null) {
                        var pr_fext_split = pr_fext.PREC[0].Split(' ');
                        ext1 = pr_fext_split[1];
                        ext2 = pr_fext_split[2];
                        extFound = true;
                    }
                }

                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.UID == uid);
                if (pr_st_cps != null && pr_st_cps.Port != Globals.NULL_PORT) {
                    port = pr_st_cps.Port;
                    portFound = true;
                }

                if (!portFound) {
                    var pr_port_uid = Database.PR_PORT_UIDs.Find(a => a.UID == uid);
                    if (pr_port_uid != null && pr_port_uid.Port != Globals.NULL_PORT) {
                        port = pr_port_uid.Port;
                        portFound = true;
                    }
                }

                var fix = new StringBuilder();
                fix.AppendLine($"prec pr_button l l0x{uid} h0x1");
                fix.AppendLine($"prec pr_button d l0x{uid} h0x1");
                fix.AppendLine($"prec pr_st_cps l l0x{uid}");
                fix.AppendLine($"prec pr_st_cps d l0x{uid}");
                fix.AppendLine($"prec pr_lwcuser d l0x{uid}");
                fix.AppendLine($"prec pr_pl_ad d l0x{uid} 1");
                fix.AppendLine($"prec pr_pl_ad d l0x{uid} 2");
                fix.AppendLine($"prec pr_pl_ad d l0x{uid} 3");
                fix.AppendLine($"prec pr_ad_user d l0x{uid}");
                fix.AppendLine($"prec pr_udata d l0x{uid}");
                fix.AppendLine($"prec pr_rjc_stn d l0x{uid}");
                fix.AppendLine($"prec pr_stn d l0x{uid}");
                fix.AppendLine($"prec pr_ttiset d l0x{uid}");
                fix.AppendLine($"prec pr_ttitype d l0x{uid}");
                fix.AppendLine($"prec pr_fext d l0x{ext1} l0x{ext2}");

                if (extFound) {
                    fix.AppendLine($"prec pr_ext d l0x{ext1} l0x{ext2}");                    
                }

                if (portFound) {
                    fix.AppendLine($"prec pr_port_uid d l0x{port}");
                    fix.AppendLine($"prec pr_moport d l0 l0 l0x{port}");
                }

                AddFix(fix.ToString());
                return fix.ToString();
            }

            // This method is for removing a PR_BRIDGE record
            internal static string PR_BRIDGE(PR_BRIDGE pr_bridge) {
                var fix = new StringBuilder();
                fix.AppendLine($"prec pr_bridge l l0x{pr_bridge.PrimaryUID} l0x{pr_bridge.BridgedUID} l0x{pr_bridge.BridgeID}");
                fix.AppendLine($"prec pr_bridge d l0x{pr_bridge.PrimaryUID} l0x{pr_bridge.BridgedUID} l0x{pr_bridge.BridgeID}");
                AddFix(fix.ToString());
                return fix.ToString();
            }

            // This method is for removing a PR_BUTTON record
            internal static string PR_BUTTON(string uid, string number) {
                var fix = $"prec pr_button l l0x{uid} h0x{number}{Environment.NewLine}prec pr_button d l0x{uid} h0x{number}";
                AddFix(fix);
                return fix;
            }

            // This method is for removing a PR_EXT record
            internal static string PR_EXT(string digits) {
                // Get the PR_EXT record
                var pr_ext = Database.PR_EXTs.Find(a => a.Digits == digits);
                
                // Generate the fix command
                var fix = $"prec pr_ext d {pr_ext.PRECFields()}";

                // Add the fix to the list and return it
                AddFix(fix);
                return fix;
            }

            // This method is for removing a PR_MOPORT record
            internal static string PR_MOPORT(string port) {
                // Generate the fix line, add it to the list, and return it
                var fix = $"prec pr_moport d l0 l0 l0x{port}";
                AddFix(fix);
                return fix;
            }

            // This method is for removing a PR_PORT_UID record
            internal static string PR_PORT_UID(string port) {
                // Generate the fix line
                var fix = $"prec pr_port_uid d l0x{port}";

                // Add the fix to the list and return it
                AddFix(fix);
                return fix;
            }

            // This method is for removing a PR_ST_CPS record
            internal static string PR_ST_CPS(string uid) {
                // Create the fix, add it to the list, and return it
                var fix = new StringBuilder();
                fix.AppendLine($"prec pr_st_cps l l0x{uid}");
                fix.AppendLine($"prec pr_st_cps d l0x{uid}");
                AddFix(fix.ToString());
                return fix.ToString();
            }

            // This method is for removing a PR_XMAP record
            internal static string PR_XMAP(PR_XMAP pr_xmap) {
                // Split the PREC line, create the fix, add it to the list, and return it
                var split = pr_xmap.PREC[0].Split(' ');
                var fix = $"prec pr_xmap d l0x{split[1]} l0x{split[2]} l0x{split[3]} l0x{split[4]} l0x{split[5]} l0x{split[6]}";
                AddFix(fix);
                return fix;
            }

            // This method is for removing a PR_OPT_STN record
            internal static string PR_OPT_STN(PR_OPT_STN pr_opt_stn) {
                // Split the PREC line, create the fix, add it to the list, and return it
                var split = pr_opt_stn.PREC[0].Split(' ');
                var fix = $"prec pr_opt_stn d l0x{split[1]} l0x{split[2]} l0x{split[3]}";
                AddFix(fix);
                return fix;
            }

            internal static string PR_FEXT(PR_FEXT pr_fext) {
                var split = pr_fext.PREC[0].Split(' ');
                var fix = $"prec pr_fext d l0x{split[1]} l0x{split[2]} l0x{split[3]}";
                AddFix(fix);
                return fix;
            }
        }

        // Class of methods used for generating update commands
        internal static class Update {
            // This method will update a PR_AMW record
            internal static string PR_AMW(PR_AMW pr_amw) {
                var fields = pr_amw.PREC[0].Split(' ');
                var fix = $"prec pr_amw d l0x{fields[1]} l0x{fields[2]} l0x{fields[3]}{Environment.NewLine}";
                var highbit = pr_amw.IsMWI ? "0" : "8";     // We are reversing what is already there
                fields[3] = highbit + fields[3].Remove(0, 1);
                fix += $"prec pr_amw a l0x{fields[1]} l0x{fields[2]} l0x{fields[3]}";
                AddFix(fix);
                return fix;
            }

            internal static string PR_AMW_REMOVE(PR_AMW pr_amw) {
                var fields = pr_amw.PREC[0].Split(' ');
                var fix = $"prec pr_amw d l0x{fields[1]} l0x{fields[2]} l0x{fields[3]}";
                AddFix(fix);
                return fix;
            }

            // This method will update a PR_ST_CPS record
            internal static string PR_ST_CPS(string uid) {
                // Declare a variable for storing the fix
                var fix = "";

                // Get the port
                var port = Database.PR_PORT_UIDs.Find(a => a.UID == uid);

                // Make sure we have a valid port
                if (port != null) {
                    fix = $"prec pr_st_cps l l0x{uid}{Environment.NewLine}prec pr_st_cps w l0x{uid} l0x{port.Port} l0x0 l0x0 l0x0";
                    AddFix(fix);
                } else {
                    var pr_stn = Database.PR_STNs.Find(a => a.UID == uid);
                    if (pr_stn.IsIP()) {
                        fix = Add.NewIPPort(uid);
                    } else {
                        fix = $"** UNABLE TO ASSIGN NEW DCP PORT AUTOMATICALLY **{Environment.NewLine}** MANUAL FIX REQUIRED **";
                        Audits.ManualFixes++;
                    }
                }
                
                return fix;
            }

            // This method will update a PR_ST_CPS record with a specific port
            internal static string PR_ST_CPS(string uid, string port) {
                // Declare a variable for storing the fix
                var fix = "";
                fix = $"prec pr_st_cps l l0x{uid}{Environment.NewLine}prec pr_st_cps w l0x{uid} l0x{port} l0x0 l0x0 l0x0";
                AddFix(fix);
                return fix;
            }

            // This method will update a PR_PORT_UID record
            internal static string PR_PORT_UID(string port, string uid) {
                // Create the fix line, add it to the list, and return it
                var fix = $"prec pr_port_uid w l0x{port} l0x{uid}";
                AddFix(fix);
                return fix;
            }

            // This method is used to update a PR_FEXT record
            internal static string PR_FEXT(string uid) {
                // Get the PR_EXT record, create the fix, add it to the list, and return it
                var pr_ext = Database.PR_EXTs.Find(a => a.UID == uid);
                var split = pr_ext.PREC[0].Split(' ');
                var fix = $"prec pr_fext w l0x{split[1]} l0x{split[2]} l0x{split[3]} l0x{split[4]}";
                AddFix(fix);
                return fix;
            }
        }
    }
}
