/*
 * Auditor3 :: Auditor
 * 
 * This class defines the primary auditor process that runs the audits against the database to
 * check for corruption issues.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.IO;
using System.Text;

namespace Auditor3 {
    internal static class Auditor {
        // This method is used to start the auditor
        internal static void Start() {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.AUDIT;
            Globals.START_TIME = DateTime.Now;
            Globals.GUI.AddStatus("Corruption audit is starting");

            // Add the header to the report
            Globals.GUI.AddOutput($"Corruption Audit v{Globals.VERSION()}");
            Globals.GUI.AddOutput("");
            Globals.GUI.AddOutput($"CM_RELEASE    : {Globals.CM_RELEASE}");
            Globals.GUI.AddOutput($"STATIONS      : {Globals.STATION_AUDITS}");
            Globals.GUI.AddOutput($"TRUNKS        : {Globals.TRUNK_AUDITS}");
            Globals.GUI.AddOutput($"ANNOUNCEMENTS : {Globals.ANNOUNCEMENT_AUDITS}");
            Globals.GUI.AddOutput($"START TIME    : {Globals.TIMESTAMP()}");
            Globals.GUI.AddOutput("");

            // Failsafe try block for catching unexpected exceptions
            try {
                // Initialize the database and fixer
                Database.Initialize();
                Fixer.Initialize();

                // Start the PREC parser
                PRECParser.Start();

                // Process the missing station types and show the PREC totals
                //Database.ProcessMissingStationTypes();
                Database.ShowTotals();

                // Run the audits
                Run();
            } catch (IndexOutOfRangeException error) {
                Globals.GUI.Error("Error during parsing : Do you have the correct CM version selected", error);
                Globals.GUI.Error("Potential CM 7.1/8.0 known issue with Collector - please pull PRECs manually");
            }
            catch (Exception error) {
                Globals.GUI.Error("Excpetion occured during audit processing", error);
            }

            // Add a note to the report if the process was cancelled
            if (Globals.CANCEL) Globals.GUI.AddOutput("** OPERATION WAS CANCELLED BY USER **");
            else {
                Fixer.GenerateFixscript();
                Globals.AUDIT_COMPLETE = true;
            }

            if (Globals.CM_RELEASE == CMRelease.CM8_1 && (Audits.AuditS31Hits != 0 || Audits.AuditS32Hits != 0 || Audits.AuditS33Hits != 0)) {
                Globals.GUI.AddOutput("");
                Globals.GUI.AddOutput("CM 8.1 PR_AMW CORRUPTION ISSUE");
                Globals.GUI.AddOutput("==============================");
                Globals.GUI.AddOutput("- Please utilize the corrective patch ");
                Globals.GUI.AddOutput("");
                Globals.GUI.AddOutput("");
            }

            // Get the runtime and add an output message
            var runtime = (DateTime.Now - Globals.START_TIME).TotalSeconds;
            Globals.GUI.AddOutput($"Audit completed in {runtime} seconds");

            // Check for manual fixes and generate the lists
            if (Database.ManualPorts.Count > 0) {
                var manualportreport = Globals.REPORT("manual_ports");
                var manualports = new StreamWriter(manualportreport);
                foreach (var manualport in Database.ManualPorts) manualports.WriteLine(manualport);
                manualports.Close();
                Globals.GUI.AddStatus($"Manual fix port list generated at {manualportreport}");
            }

            if (Database.ManualUIDs.Count > 0) {
                var manualuidreport = Globals.REPORT("manual_uids");
                var manualuids = new StreamWriter(manualuidreport);
                foreach (var manualuid in Database.ManualUIDs) manualuids.WriteLine(manualuid);
                manualuids.Close();
                Globals.GUI.AddStatus($"Manual fix UID list generated at {manualuidreport}");
            }

            // Write the report
            var output = Globals.GUI.GetOutput();
            var report = Globals.REPORT("audit");
            var writer = new StreamWriter(report);
            writer.Write(output);
            writer.Close();

            // Add a status message and go idle
            Globals.GUI.AddStatus($"Audit report generated at {report}");
            Globals.GUI.Idle();
        }

        // This method runs the audit loops
        private static void Run() {
            // Validate all the required PRECs are loaded
            if (!Database.ValidatePRECs()) return;

            // Run the requested audit loops
            if (Globals.STATION_AUDITS && !Globals.CANCEL) StationAudits();
            if (Globals.TRUNK_AUDITS && !Globals.CANCEL) TrunkAudits();
            if (Globals.ANNOUNCEMENT_AUDITS && !Globals.CANCEL) AnnouncementAudits();

            // Show the counter totals
            Audits.ShowCounters();
        }

        // This method runs the station audit loops
        private static void StationAudits() {
            // Set values for the PR_STN audit loop
            Globals.PROCESS = Process.PR_AMW_LOOP;
            Audits.ToCheck = Database.PR_AMWs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_AMW audit loop is starting");

            // Do AuditS31
            Audits.AuditS31();

            // Loop through the records now
            foreach (var pr_amw in Database.PR_AMWs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                Audits.AuditS32(pr_amw);
                Audits.AuditS33(pr_amw);
            }

            // Set values for the PR_STN audit loop
            Globals.PROCESS = Process.PR_STN_LOOP;
            Audits.ToCheck = Database.PR_STNs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_STN audit loop is starting");

            // Loop through the records
            foreach (var pr_stn in Database.PR_STNs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_stn.IsAuditable) continue;

                // Run the audits
                if (!Audits.AuditS01(pr_stn)) continue;
                if (!Audits.AuditS04(pr_stn)) continue;
                if (!Audits.AuditS29(pr_stn)) continue;
                if (!Audits.AuditS10(pr_stn)) continue;
                if (!Audits.AuditS27(pr_stn)) continue;
                Audits.AuditS28(pr_stn);
            }

            // Set values for the PR_UDATA loop
            Globals.PROCESS = Process.PR_UDATA_LOOP;
            Audits.ToCheck = Database.PR_UDATAs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_UDATA audit loop is starting");

            // Loop through the records
            foreach (var pr_udata in Database.PR_UDATAs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_udata.IsAuditable) continue;

                // Run the audits
                Audits.AuditS22(pr_udata);
            }

            // Set values for the PR_PORT_UID loop
            Globals.PROCESS = Process.PR_PORT_UID_LOOP;
            Audits.ToCheck = Database.PR_PORT_UIDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_PORT_UID audit loop is starting");

            // Loop through the records
            foreach (var pr_port_uid in Database.PR_PORT_UIDs) {
                // Pre-checked
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_port_uid.IsAuditable) continue;

                // Run through the audits
                if (!Audits.AuditS21(pr_port_uid)) continue;
                if (!Audits.AuditS06(pr_port_uid)) continue;
                if (!Audits.AuditS07(pr_port_uid)) continue;
                if (!Audits.AuditS08(pr_port_uid)) continue;
                if (!Audits.AuditS09(pr_port_uid)) continue;
                if (!Audits.AuditS15(pr_port_uid)) continue;
                if (!Audits.AuditS16(pr_port_uid)) continue;
                if (!Audits.AuditS17(pr_port_uid)) continue;                
            }

            // Set values for the PR_ST_CPS loop
            Globals.PROCESS = Process.PR_ST_CPS_LOOP;
            Audits.ToCheck = Database.PR_ST_CPSs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_ST_CPS audit loop is starting");

            // Loop through the records
            foreach (var pr_st_cps in Database.PR_ST_CPSs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (pr_st_cps.Port == Globals.NULL_PORT) continue;

                // Run the audits
                if (!Audits.AuditS02(pr_st_cps)) continue;
                if (!Audits.AuditS03(pr_st_cps)) continue;
                if (!Audits.AuditS12(pr_st_cps)) continue;
                if (!Audits.AuditS20(pr_st_cps)) continue;
                if (!Audits.AuditS30(pr_st_cps)) continue;
            }

            // Set values for the PR_MOPORT loop
            Globals.PROCESS = Process.PR_MOPORT_LOOP;
            Audits.ToCheck = Database.PR_MOPORTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_MOPORT audit loop is starting");

            // Loop through the records
            foreach (var pr_moport in Database.PR_MOPORTs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_moport.IsAuditable) continue;

                // Run the audits
                if (!Audits.AuditS13(pr_moport)) continue;
                Audits.AuditS14(pr_moport);
            }

            // Set values for the PR_EXT loop
            Globals.PROCESS = Process.PR_EXT_LOOP;
            Audits.ToCheck = Database.PR_EXTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_EXT audit loop is starting");

            // Loop through the records
            foreach (var pr_ext in Database.PR_EXTs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditS05(pr_ext);
            }

            // Set values for the PR_FEXT loop
            Globals.PROCESS = Process.PR_FEXT_LOOP;
            Audits.ToCheck = Database.PR_FEXTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_FEXT audit loop is starting");
            
            // Loop through the records
            foreach (var pr_fext in Database.PR_FEXTs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                // Audits.AuditS34(pr_fext);
                // Audits.AuditS35(pr_fext);
            }

            // Set values for the PR_BUTTON loop
            Globals.PROCESS = Process.PR_BUTTON_LOOP;
            Audits.ToCheck = Database.PR_BUTTONs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_BUTTON audit loop is starting");

            // Loop through the records
            foreach (var pr_button in Database.PR_BUTTONs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                if (!Audits.AuditS11(pr_button)) continue;
                Audits.AuditS18(pr_button);
            }

            // Set values for the PR_BRIDGE loop
            Globals.PROCESS = Process.PR_BRIDGE_LOOP;
            Audits.ToCheck = Database.PR_BRIDGEs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_BRIDGE audit loop is starting");

            // Loop through the records
            foreach (var pr_bridge in Database.PR_BRIDGEs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditS19(pr_bridge);
            }

            // Set values for the PR_XMAP loop
            Globals.PROCESS = Process.PR_XMAP_LOOP;
            Audits.ToCheck = Database.PR_XMAPs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_XMAP audit loop is starting");

            // Loop through the records
            foreach (var pr_xmap in Database.PR_XMAPs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                if (!Audits.AuditS25(pr_xmap)) continue;
                Audits.AuditS23(pr_xmap);
            }

            // Set values for the PR_OPT_STN loop
            Globals.PROCESS = Process.PR_OPT_STN_LOOP;
            Audits.ToCheck = Database.PR_OPT_STNs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_OPT_STN audit loop is starting");

            // Loop through the records
            foreach (var pr_opt_stn in Database.PR_OPT_STNs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                if (!Audits.AuditS24(pr_opt_stn)) continue;
                Audits.AuditS26(pr_opt_stn);
            }
        }

        // This method runs the trunk audit loops
        private static void TrunkAudits() {
            
            // Set values for the PR_ACD_TRUNK audit loop
            Globals.PROCESS = Process.PR_ACD_TRUNK_LOOP;
            Audits.ToCheck = Database.PR_ACD_TRUNKs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_ACD_TRUNK_LOOP loop is starting");

            foreach (var pr_acd_trunk in Database.PR_ACD_TRUNKs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (pr_acd_trunk.Flagged) continue;

                // Run the audits
                Audits.AuditT04(pr_acd_trunk);
            }

            // Set values for the PR_MOPORT audit loop
            Globals.PROCESS = Process.PR_MOPORT_LOOP;
            Audits.ToCheck = Database.PR_MOPORTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_MOPORT_LOOP loop is starting");

            foreach (var pr_moport in Database.PR_MOPORTs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_moport.IsAuditable) continue;

                if (!Audits.AuditT07(pr_moport)) continue;
                if (!Audits.AuditT09(pr_moport)) continue;
            }

            // Set values for the PR_PORT_UID audit loop
            Globals.PROCESS = Process.PR_PORT_UID_LOOP;
            Audits.ToCheck = Database.PR_PORT_UIDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_PORT_UID_LOOP loop is starting");

            foreach (var pr_port_uid in Database.PR_PORT_UIDs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_port_uid.IsTrunk) continue;

                if (!Audits.AuditT08(pr_port_uid)) continue;
            }

            // Set values for the PR_TR_GRP audit loop
            Globals.PROCESS = Process.PR_TR_GRP_LOOP;
            Audits.ToCheck = Database.PR_TR_GRPs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_TR_GRP loop is starting");

            foreach (var pr_tr_grp in Database.PR_TR_GRPs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditT05(pr_tr_grp);

                // Get all the members for the group and audit them
                var pr_tr_mbrs = Database.PR_TR_MBRs.FindAll(a => a.TrunkGroup == pr_tr_grp.UID);
                foreach (var pr_tr_mbr in pr_tr_mbrs) {
                    if (!Audits.AuditT01(pr_tr_mbr)) continue;
                    Audits.AuditT06(pr_tr_grp.Measured, pr_tr_mbr);
                }

                // Get all the trunks for the group and audit them
                var pr_trunks = Database.PR_TRUNKs.FindAll(a => a.TrunkGroup == pr_tr_grp.UID);
                foreach (var pr_trunk in pr_trunks) {
                    if (!Audits.AuditT02(pr_trunk)) continue;
                    Audits.AuditT03(pr_trunk);
                }
            }
        }

        // This method runs the announcement audit loops
        private static void AnnouncementAudits() {
            // Set values for the PR_INT_ANNC audit loop
            Globals.PROCESS = Process.PR_INT_ANNC_LOOP;
            Audits.ToCheck = Database.PR_INT_ANNCs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_INT_ANNC audit loop is starting");

            // Loop through the records
            foreach (var pr_int_annc in Database.PR_INT_ANNCs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                if (!Audits.AuditA09(pr_int_annc)) continue;
                if (!Audits.AuditA01(pr_int_annc)) continue;
                if (!Audits.AuditA02(pr_int_annc)) continue;
                if (!Audits.AuditA05(pr_int_annc)) continue;
                if (!Audits.AuditA06(pr_int_annc)) continue;
                Audits.AuditA08(pr_int_annc);
            }

            // Set values for the PR_IANC_BD audit loop
            Globals.PROCESS = Process.PR_IANC_BD_LOOP;
            Audits.ToCheck = Database.PR_IANC_BDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_IANC_BD audit loop is starting");

            // Loop through the records
            foreach (var pr_ianc_bd in Database.PR_IANC_BDs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditA03(pr_ianc_bd);
            }

            // Set values for the PR_IANC_BD audit loop
            Globals.PROCESS = Process.PR_GM_IANC_BD_LOOP;
            Audits.ToCheck = Database.PR_GM_IANC_BDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_GM_IANC_BD audit loop is starting");

            // Loop through the records
            foreach (var pr_gm_ianc_bd in Database.PR_GM_IANC_BDs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditA10(pr_gm_ianc_bd);
            }

            // Set values for the PR_EXT loop
            Globals.PROCESS = Process.PR_EXT_LOOP;
            Audits.ToCheck = Database.PR_EXTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_EXT audit loop is starting");

            // Loop through the records
            foreach (var pr_ext in Database.PR_EXTs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditA04(pr_ext);
            }

            // Set values for the PR_UDATA loop
            Globals.PROCESS = Process.PR_UDATA_LOOP;
            Audits.ToCheck = Database.PR_UDATAs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_UDATA audit loop is starting");

            // Loop through the records
            foreach (var pr_udata in Database.PR_UDATAs) {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                Audits.AuditA07(pr_udata);
            }

            // Set values for the PR_UDATA loop
            Globals.PROCESS = Process.PR_AUDIO_GRP_LOOP;
            Audits.ToCheck = Database.PR_AUDIO_GRPs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_AUDIO_GRP audit loop is starting");

            // COMPLICATED AUDIT LOOP - PLEASE BE CAREFUL WITH CHANGES
            foreach (var pr_audio_grp in Database.PR_AUDIO_GRPs) {
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // We need the UID of an announcement using this group, lets find one
                var pr_int_annc = Database.PR_INT_ANNCs.Find(a => a.AudioGroup == pr_audio_grp.AudioGroup);

                // Make sure we have one
                if (pr_int_annc == null) continue;

                // Now, get all the boards that use this UID
                var pr_gm_ianc_bds = Database.PR_GM_IANC_BDs.FindAll(a => a.UID == pr_int_annc.UID);

                // Loop through these boards
                foreach (var pr_gm_ianc_bd in pr_gm_ianc_bds) {
                    Audits.AuditA11(pr_gm_ianc_bd, pr_audio_grp.AudioGroup);
                }
            }
        }
    }
}
