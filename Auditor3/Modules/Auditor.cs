/*
 * Auditor3 :: Auditor
 * 
 * This class defines the primary auditor process that runs the audits against the database to
 * check for corruption issues.
 * 
 * REFACTORED: Uses strategy pattern via AuditStrategies accessor while maintaining original audit flow
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using Auditor3.Models;
using Auditor3.Services;
using System;
using System.IO;
using System.Text;

namespace Auditor3
{
    internal static class Auditor
    {

        // This method is used to start the auditor
        internal static void Start()
        {
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

            System.Diagnostics.Debug.WriteLine("✅ REFACTORED: Auditor.Start() - Using strategy pattern");

            // Failsafe try block for catching unexpected exceptions
            try
            {
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
            }
            catch (IndexOutOfRangeException error)
            {
                Globals.GUI.Error("Error during parsing : Do you have the correct CM version selected", error);
                Globals.GUI.Error("Potential CM 7.1/8.0 known issue with Collector - please pull PRECs manually");
            }
            catch (Exception error)
            {
                Globals.GUI.Error("Excpetion occured during audit processing", error);
            }

            // Add a note to the report if the process was cancelled
            if (Globals.CANCEL) Globals.GUI.AddOutput("** OPERATION WAS CANCELLED BY USER **");
            else
            {
                Fixer.GenerateFixscript();
                Globals.AUDIT_COMPLETE = true;
            }

            if (Globals.CM_RELEASE == CMRelease.CM8_1 && (Audits.AuditS31Hits != 0 || Audits.AuditS32Hits != 0 || Audits.AuditS33Hits != 0))
            {
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
            if (Database.ManualPorts.Count > 0)
            {
                var manualportreport = Globals.REPORT("manual_ports");
                var manualports = new StreamWriter(manualportreport);
                foreach (var manualport in Database.ManualPorts) manualports.WriteLine(manualport);
                manualports.Close();
                Globals.GUI.AddStatus($"Manual fix port list generated at {manualportreport}");
            }

            if (Database.ManualUIDs.Count > 0)
            {
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
        private static void Run()
        {
            // Validate all the required PRECs are loaded
            if (!Database.ValidatePRECs()) return;

            // Run the requested audit loops
            if (Globals.STATION_AUDITS && !Globals.CANCEL) StationAudits();
            if (Globals.TRUNK_AUDITS && !Globals.CANCEL) TrunkAudits();
            if (Globals.ANNOUNCEMENT_AUDITS && !Globals.CANCEL) AnnouncementAudits();

            // Show the counter totals
            Audits.ShowCounters();
        }

        // Helper method to increment audit-specific hit counters
        private static void IncrementAuditHits(string auditCode)
        {
            switch (auditCode)
            {
                case "AUDIT-S01": Audits.AuditS01Hits++; break;
                case "AUDIT-S02": Audits.AuditS02Hits++; break;
                case "AUDIT-S03": Audits.AuditS03Hits++; break;
                case "AUDIT-S04": Audits.AuditS04Hits++; break;
                case "AUDIT-S05": Audits.AuditS05Hits++; break;
                case "AUDIT-S06": Audits.AuditS06Hits++; break;
                case "AUDIT-S07": Audits.AuditS07Hits++; break;
                case "AUDIT-S08": Audits.AuditS08Hits++; break;
                case "AUDIT-S09": Audits.AuditS09Hits++; break;
                case "AUDIT-S10": Audits.AuditS10Hits++; break;
                case "AUDIT-S11": Audits.AuditS11Hits++; break;
                case "AUDIT-S12": Audits.AuditS12Hits++; break;
                case "AUDIT-S13": Audits.AuditS13Hits++; break;
                case "AUDIT-S14": Audits.AuditS14Hits++; break;
                case "AUDIT-S15": Audits.AuditS15Hits++; break;
                case "AUDIT-S16": Audits.AuditS16Hits++; break;
                case "AUDIT-S17": Audits.AuditS17Hits++; break;
                case "AUDIT-S18": Audits.AuditS18Hits++; break;
                case "AUDIT-S19": Audits.AuditS19Hits++; break;
                case "AUDIT-S20": Audits.AuditS20Hits++; break;
                case "AUDIT-S21": Audits.AuditS21Hits++; break;
                case "AUDIT-S22": Audits.AuditS22Hits++; break;
                case "AUDIT-S23": Audits.AuditS23Hits++; break;
                case "AUDIT-S24": Audits.AuditS24Hits++; break;
                case "AUDIT-S25": Audits.AuditS25Hits++; break;
                case "AUDIT-S26": Audits.AuditS26Hits++; break;
                case "AUDIT-S27": Audits.AuditS27Hits++; break;
                case "AUDIT-S28": Audits.AuditS28Hits++; break;
                case "AUDIT-S29": Audits.AuditS29Hits++; break;
                case "AUDIT-S30": Audits.AuditS30Hits++; break;
                case "AUDIT-S31": Audits.AuditS31Hits++; break;
                case "AUDIT-S32": Audits.AuditS32Hits++; break;
                case "AUDIT-S33": Audits.AuditS33Hits++; break;
                case "AUDIT-S34": Audits.AuditS34Hits++; break;
                case "AUDIT-S35": Audits.AuditS35Hits++; break;
                case "AUDIT-A01": Audits.AuditA01Hits++; break;
                case "AUDIT-A02": Audits.AuditA02Hits++; break;
                case "AUDIT-A03": Audits.AuditA03Hits++; break;
                case "AUDIT-A04": Audits.AuditA04Hits++; break;
                case "AUDIT-A05": Audits.AuditA05Hits++; break;
                case "AUDIT-A06": Audits.AuditA06Hits++; break;
                case "AUDIT-A07": Audits.AuditA07Hits++; break;
                case "AUDIT-A08": Audits.AuditA08Hits++; break;
                case "AUDIT-A09": Audits.AuditA09Hits++; break;
                case "AUDIT-A10": Audits.AuditA10Hits++; break;
                case "AUDIT-A11": Audits.AuditA11Hits++; break;
                case "AUDIT-T01": Audits.AuditT01Hits++; break;
                case "AUDIT-T02": Audits.AuditT02Hits++; break;
                case "AUDIT-T03": Audits.AuditT03Hits++; break;
                case "AUDIT-T04": Audits.AuditT04Hits++; break;
                case "AUDIT-T05": Audits.AuditT05Hits++; break;
                case "AUDIT-T06": Audits.AuditT06Hits++; break;
                case "AUDIT-T07": Audits.AuditT07Hits++; break;
                case "AUDIT-T08": Audits.AuditT08Hits++; break;
                case "AUDIT-T09": Audits.AuditT09Hits++; break;
            }
        }

        // Helper method to handle audit failure with counter increments
        private static void HandleAuditFailure(string auditCode, string message, string fixScript, bool isManualFix, AuditCategory category)
        {
            Globals.GUI.AddOutput(message);
            Audits.Corrupted++;

            if (category == AuditCategory.Station)
            {
                Audits.CorruptedStations++;
            }
            else if (category == AuditCategory.Trunk)
            {
                Audits.CorruptedTrunks++;
            }
            else if (category == AuditCategory.Announcement)
            {
                Audits.CorruptedAnnouncements++;
            }

            if (isManualFix)
            {
                Audits.ManualFixes++;
            }

            IncrementAuditHits(auditCode);
        }

        // This method runs the station audit loops
        private static void StationAudits()
        {
            // Set values for the PR_AMW audit loop
            Globals.PROCESS = Process.PR_AMW_LOOP;
            Audits.ToCheck = Database.PR_AMWs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_AMW audit loop is starting");

            // Do AuditS31
            AuditStrategies.AuditS31();

            // Loop through the records now
            foreach (var pr_amw in Database.PR_AMWs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                var result32 = AuditStrategies.AuditS32(pr_amw);
                if (!result32.Passed)
                {
                    HandleAuditFailure("AUDIT-S32", result32.Message, result32.FixScript, result32.RequiresManualFix, AuditCategory.Station);
                }

                var result33 = AuditStrategies.AuditS33(pr_amw);
                if (!result33.Passed)
                {
                    HandleAuditFailure("AUDIT-S33", result33.Message, result33.FixScript, result33.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_STN audit loop
            Globals.PROCESS = Process.PR_STN_LOOP;
            Audits.ToCheck = Database.PR_STNs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_STN audit loop is starting");

            // Loop through the records
            foreach (var pr_stn in Database.PR_STNs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_stn.IsAuditable) continue;

                // Run the audits with ORIGINAL CONTROL FLOW (early exit on failure)
                var result01 = AuditStrategies.AuditS01(pr_stn);
                if (!result01.Passed)
                {
                    HandleAuditFailure("AUDIT-S01", result01.Message, result01.FixScript, result01.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result04 = AuditStrategies.AuditS04(pr_stn);
                if (!result04.Passed)
                {
                    HandleAuditFailure("AUDIT-S04", result04.Message, result04.FixScript, result04.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result29 = AuditStrategies.AuditS29(pr_stn);
                if (!result29.Passed)
                {
                    HandleAuditFailure("AUDIT-S29", result29.Message, result29.FixScript, result29.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result10 = AuditStrategies.AuditS10(pr_stn);
                if (!result10.Passed)
                {
                    HandleAuditFailure("AUDIT-S10", result10.Message, result10.FixScript, result10.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result27 = AuditStrategies.AuditS27(pr_stn);
                if (!result27.Passed)
                {
                    HandleAuditFailure("AUDIT-S27", result27.Message, result27.FixScript, result27.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result28 = AuditStrategies.AuditS28(pr_stn);
                if (!result28.Passed)
                {
                    HandleAuditFailure("AUDIT-S28", result28.Message, result28.FixScript, result28.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_UDATA loop
            Globals.PROCESS = Process.PR_UDATA_LOOP;
            Audits.ToCheck = Database.PR_UDATAs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_UDATA audit loop is starting");

            // Loop through the records
            foreach (var pr_udata in Database.PR_UDATAs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_udata.IsAuditable) continue;

                // Run the audits
                var result22 = AuditStrategies.AuditS22(pr_udata);
                if (!result22.Passed)
                {
                    HandleAuditFailure("AUDIT-S22", result22.Message, result22.FixScript, result22.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_PORT_UID loop
            Globals.PROCESS = Process.PR_PORT_UID_LOOP;
            Audits.ToCheck = Database.PR_PORT_UIDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_PORT_UID audit loop is starting");

            // Loop through the records
            foreach (var pr_port_uid in Database.PR_PORT_UIDs)
            {
                // Pre-checked
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_port_uid.IsAuditable) continue;

                // Run through the audits
                var result21 = AuditStrategies.AuditS21(pr_port_uid);
                if (!result21.Passed)
                {
                    HandleAuditFailure("AUDIT-S21", result21.Message, result21.FixScript, result21.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result06 = AuditStrategies.AuditS06(pr_port_uid);
                if (!result06.Passed)
                {
                    HandleAuditFailure("AUDIT-S06", result06.Message, result06.FixScript, result06.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result07 = AuditStrategies.AuditS07(pr_port_uid);
                if (!result07.Passed)
                {
                    HandleAuditFailure("AUDIT-S07", result07.Message, result07.FixScript, result07.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result08 = AuditStrategies.AuditS08(pr_port_uid);
                if (!result08.Passed)
                {
                    HandleAuditFailure("AUDIT-S08", result08.Message, result08.FixScript, result08.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result09 = AuditStrategies.AuditS09(pr_port_uid);
                if (!result09.Passed)
                {
                    HandleAuditFailure("AUDIT-S09", result09.Message, result09.FixScript, result09.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result15 = AuditStrategies.AuditS15(pr_port_uid);
                if (!result15.Passed)
                {
                    HandleAuditFailure("AUDIT-S15", result15.Message, result15.FixScript, result15.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result16 = AuditStrategies.AuditS16(pr_port_uid);
                if (!result16.Passed)
                {
                    HandleAuditFailure("AUDIT-S16", result16.Message, result16.FixScript, result16.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result17 = AuditStrategies.AuditS17(pr_port_uid);
                if (!result17.Passed)
                {
                    HandleAuditFailure("AUDIT-S17", result17.Message, result17.FixScript, result17.RequiresManualFix, AuditCategory.Station);
                    continue;
                }
            }

            // Set values for the PR_ST_CPS loop
            Globals.PROCESS = Process.PR_ST_CPS_LOOP;
            Audits.ToCheck = Database.PR_ST_CPSs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_ST_CPS audit loop is starting");

            // Loop through the records
            foreach (var pr_st_cps in Database.PR_ST_CPSs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (pr_st_cps.Port == Globals.NULL_PORT) continue;

                // Run the audits
                var result02 = AuditStrategies.AuditS02(pr_st_cps);
                if (!result02.Passed)
                {
                    HandleAuditFailure("AUDIT-S02", result02.Message, result02.FixScript, result02.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result03 = AuditStrategies.AuditS03(pr_st_cps);
                if (!result03.Passed)
                {
                    HandleAuditFailure("AUDIT-S03", result03.Message, result03.FixScript, result03.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result12 = AuditStrategies.AuditS12(pr_st_cps);
                if (!result12.Passed)
                {
                    HandleAuditFailure("AUDIT-S12", result12.Message, result12.FixScript, result12.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result20 = AuditStrategies.AuditS20(pr_st_cps);
                if (!result20.Passed)
                {
                    HandleAuditFailure("AUDIT-S20", result20.Message, result20.FixScript, result20.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result30 = AuditStrategies.AuditS30(pr_st_cps);
                if (!result30.Passed)
                {
                    HandleAuditFailure("AUDIT-S30", result30.Message, result30.FixScript, result30.RequiresManualFix, AuditCategory.Station);
                    continue;
                }
            }

            // Set values for the PR_MOPORT loop
            Globals.PROCESS = Process.PR_MOPORT_LOOP;
            Audits.ToCheck = Database.PR_MOPORTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_MOPORT audit loop is starting");

            // Loop through the records
            foreach (var pr_moport in Database.PR_MOPORTs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_moport.IsAuditable) continue;

                // Run the audits
                var result13 = AuditStrategies.AuditS13(pr_moport);
                if (!result13.Passed)
                {
                    HandleAuditFailure("AUDIT-S13", result13.Message, result13.FixScript, result13.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result14 = AuditStrategies.AuditS14(pr_moport);
                if (!result14.Passed)
                {
                    HandleAuditFailure("AUDIT-S14", result14.Message, result14.FixScript, result14.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_EXT loop
            Globals.PROCESS = Process.PR_EXT_LOOP;
            Audits.ToCheck = Database.PR_EXTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_EXT audit loop is starting");

            // Loop through the records
            foreach (var pr_ext in Database.PR_EXTs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result05 = AuditStrategies.AuditS05(pr_ext);
                if (!result05.Passed)
                {
                    HandleAuditFailure("AUDIT-S05", result05.Message, result05.FixScript, result05.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_FEXT loop
            Globals.PROCESS = Process.PR_FEXT_LOOP;
            Audits.ToCheck = Database.PR_FEXTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_FEXT audit loop is starting");

            // Loop through the records
            foreach (var pr_fext in Database.PR_FEXTs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                //// Run the audits
                //var result34 = AuditStrategies.AuditS34(pr_fext);
                //if (!result34.Passed)
                //{
                //    HandleAuditFailure("AUDIT-S34", result34.Message, result34.FixScript, result34.RequiresManualFix, AuditCategory.Station);
                //}

                //var result35 = AuditStrategies.AuditS35(pr_fext);
                //if (!result35.Passed)
                //{
                //    HandleAuditFailure("AUDIT-S35", result35.Message, result35.FixScript, result35.RequiresManualFix, AuditCategory.Station);
                //}
            }

            // Set values for the PR_BUTTON loop
            Globals.PROCESS = Process.PR_BUTTON_LOOP;
            Audits.ToCheck = Database.PR_BUTTONs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_BUTTON audit loop is starting");

            // Loop through the records
            foreach (var pr_button in Database.PR_BUTTONs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result11 = AuditStrategies.AuditS11(pr_button);
                if (!result11.Passed)
                {
                    HandleAuditFailure("AUDIT-S11", result11.Message, result11.FixScript, result11.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result18 = AuditStrategies.AuditS18(pr_button);
                if (!result18.Passed)
                {
                    HandleAuditFailure("AUDIT-S18", result18.Message, result18.FixScript, result18.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_BRIDGE loop
            Globals.PROCESS = Process.PR_BRIDGE_LOOP;
            Audits.ToCheck = Database.PR_BRIDGEs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_BRIDGE audit loop is starting");

            // Loop through the records
            foreach (var pr_bridge in Database.PR_BRIDGEs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result19 = AuditStrategies.AuditS19(pr_bridge);
                if (!result19.Passed)
                {
                    HandleAuditFailure("AUDIT-S19", result19.Message, result19.FixScript, result19.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_XMAP loop
            Globals.PROCESS = Process.PR_XMAP_LOOP;
            Audits.ToCheck = Database.PR_XMAPs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_XMAP audit loop is starting");

            // Loop through the records
            foreach (var pr_xmap in Database.PR_XMAPs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result25 = AuditStrategies.AuditS25(pr_xmap);
                if (!result25.Passed)
                {
                    HandleAuditFailure("AUDIT-S25", result25.Message, result25.FixScript, result25.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result23 = AuditStrategies.AuditS23(pr_xmap);
                if (!result23.Passed)
                {
                    HandleAuditFailure("AUDIT-S23", result23.Message, result23.FixScript, result23.RequiresManualFix, AuditCategory.Station);
                }
            }

            // Set values for the PR_OPT_STN loop
            Globals.PROCESS = Process.PR_OPT_STN_LOOP;
            Audits.ToCheck = Database.PR_OPT_STNs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_OPT_STN audit loop is starting");

            // Loop through the records
            foreach (var pr_opt_stn in Database.PR_OPT_STNs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result24 = AuditStrategies.AuditS24(pr_opt_stn);
                if (!result24.Passed)
                {
                    HandleAuditFailure("AUDIT-S24", result24.Message, result24.FixScript, result24.RequiresManualFix, AuditCategory.Station);
                    continue;
                }

                var result26 = AuditStrategies.AuditS26(pr_opt_stn);
                if (!result26.Passed)
                {
                    HandleAuditFailure("AUDIT-S26", result26.Message, result26.FixScript, result26.RequiresManualFix, AuditCategory.Station);
                }
            }
        }

        // This method runs the trunk audit loops
        private static void TrunkAudits()
        {

            // Set values for the PR_ACD_TRUNK audit loop
            Globals.PROCESS = Process.PR_ACD_TRUNK_LOOP;
            Audits.ToCheck = Database.PR_ACD_TRUNKs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_ACD_TRUNK_LOOP loop is starting");

            foreach (var pr_acd_trunk in Database.PR_ACD_TRUNKs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (pr_acd_trunk.Flagged) continue;

                // Run the audits
                var result04 = AuditStrategies.AuditT04(pr_acd_trunk);
                if (!result04.Passed)
                {
                    HandleAuditFailure("AUDIT-T04", result04.Message, result04.FixScript, result04.RequiresManualFix, AuditCategory.Trunk);
                }
            }

            // Set values for the PR_MOPORT audit loop
            Globals.PROCESS = Process.PR_MOPORT_LOOP;
            Audits.ToCheck = Database.PR_MOPORTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_MOPORT_LOOP loop is starting");

            foreach (var pr_moport in Database.PR_MOPORTs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_moport.IsAuditable) continue;

                var result07 = AuditStrategies.AuditT07(pr_moport);
                if (!result07.Passed)
                {
                    HandleAuditFailure("AUDIT-T07", result07.Message, result07.FixScript, result07.RequiresManualFix, AuditCategory.Trunk);
                    continue;
                }

                var result09 = AuditStrategies.AuditT09(pr_moport);
                if (!result09.Passed)
                {
                    HandleAuditFailure("AUDIT-T09", result09.Message, result09.FixScript, result09.RequiresManualFix, AuditCategory.Trunk);
                    continue;
                }
            }

            // Set values for the PR_PORT_UID audit loop
            Globals.PROCESS = Process.PR_PORT_UID_LOOP;
            Audits.ToCheck = Database.PR_PORT_UIDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_PORT_UID_LOOP loop is starting");

            foreach (var pr_port_uid in Database.PR_PORT_UIDs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;
                if (!pr_port_uid.IsTrunk) continue;

                var result08 = AuditStrategies.AuditT08(pr_port_uid);
                if (!result08.Passed)
                {
                    HandleAuditFailure("AUDIT-T08", result08.Message, result08.FixScript, result08.RequiresManualFix, AuditCategory.Trunk);
                    continue;
                }
            }

            // Set values for the PR_TR_GRP audit loop
            Globals.PROCESS = Process.PR_TR_GRP_LOOP;
            Audits.ToCheck = Database.PR_TR_GRPs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_TR_GRP loop is starting");

            foreach (var pr_tr_grp in Database.PR_TR_GRPs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result05 = AuditStrategies.AuditT05(pr_tr_grp);
                if (!result05.Passed)
                {
                    HandleAuditFailure("AUDIT-T05", result05.Message, result05.FixScript, result05.RequiresManualFix, AuditCategory.Trunk);
                }

                // Get all the members for the group and audit them
                var pr_tr_mbrs = Database.PR_TR_MBRs.FindAll(a => a.TrunkGroup == pr_tr_grp.UID);
                foreach (var pr_tr_mbr in pr_tr_mbrs)
                {
                    var result01 = AuditStrategies.AuditT01(pr_tr_mbr);
                    if (!result01.Passed)
                    {
                        HandleAuditFailure("AUDIT-T01", result01.Message, result01.FixScript, result01.RequiresManualFix, AuditCategory.Trunk);
                        continue;
                    }

                    var result06 = AuditStrategies.AuditT06(pr_tr_grp.Measured, pr_tr_mbr);
                    if (!result06.Passed)
                    {
                        HandleAuditFailure("AUDIT-T06", result06.Message, result06.FixScript, result06.RequiresManualFix, AuditCategory.Trunk);
                    }
                }

                // Get all the trunks for the group and audit them
                var pr_trunks = Database.PR_TRUNKs.FindAll(a => a.TrunkGroup == pr_tr_grp.UID);
                foreach (var pr_trunk in pr_trunks)
                {
                    var result02 = AuditStrategies.AuditT02(pr_trunk);
                    if (!result02.Passed)
                    {
                        HandleAuditFailure("AUDIT-T02", result02.Message, result02.FixScript, result02.RequiresManualFix, AuditCategory.Trunk);
                        continue;
                    }

                    var result03 = AuditStrategies.AuditT03(pr_trunk);
                    if (!result03.Passed)
                    {
                        HandleAuditFailure("AUDIT-T03", result03.Message, result03.FixScript, result03.RequiresManualFix, AuditCategory.Trunk);
                    }
                }
            }
        }

        // This method runs the announcement audit loops
        private static void AnnouncementAudits()
        {
            // Set values for the PR_INT_ANNC audit loop
            Globals.PROCESS = Process.PR_INT_ANNC_LOOP;
            Audits.ToCheck = Database.PR_INT_ANNCs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_INT_ANNC audit loop is starting");

            // Loop through the records
            foreach (var pr_int_annc in Database.PR_INT_ANNCs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result09 = AuditStrategies.AuditA09(pr_int_annc);
                if (!result09.Passed)
                {
                    HandleAuditFailure("AUDIT-A09", result09.Message, result09.FixScript, result09.RequiresManualFix, AuditCategory.Announcement);
                    continue;
                }

                var result01 = AuditStrategies.AuditA01(pr_int_annc);
                if (!result01.Passed)
                {
                    HandleAuditFailure("AUDIT-A01", result01.Message, result01.FixScript, result01.RequiresManualFix, AuditCategory.Announcement);
                    continue;
                }

                var result02 = AuditStrategies.AuditA02(pr_int_annc);
                if (!result02.Passed)
                {
                    HandleAuditFailure("AUDIT-A02", result02.Message, result02.FixScript, result02.RequiresManualFix, AuditCategory.Announcement);
                    continue;
                }

                var result05 = AuditStrategies.AuditA05(pr_int_annc);
                if (!result05.Passed)
                {
                    HandleAuditFailure("AUDIT-A05", result05.Message, result05.FixScript, result05.RequiresManualFix, AuditCategory.Announcement);
                    continue;
                }

                var result06 = AuditStrategies.AuditA06(pr_int_annc);
                if (!result06.Passed)
                {
                    HandleAuditFailure("AUDIT-A06", result06.Message, result06.FixScript, result06.RequiresManualFix, AuditCategory.Announcement);
                    continue;
                }

                var result08 = AuditStrategies.AuditA08(pr_int_annc);
                if (!result08.Passed)
                {
                    HandleAuditFailure("AUDIT-A08", result08.Message, result08.FixScript, result08.RequiresManualFix, AuditCategory.Announcement);
                }
            }

            // Set values for the PR_IANC_BD audit loop
            Globals.PROCESS = Process.PR_IANC_BD_LOOP;
            Audits.ToCheck = Database.PR_IANC_BDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_IANC_BD audit loop is starting");

            // Loop through the records
            foreach (var pr_ianc_bd in Database.PR_IANC_BDs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result03 = AuditStrategies.AuditA03(pr_ianc_bd);
                if (!result03.Passed)
                {
                    HandleAuditFailure("AUDIT-A03", result03.Message, result03.FixScript, result03.RequiresManualFix, AuditCategory.Announcement);
                }
            }

            // Set values for the PR_IANC_BD audit loop
            Globals.PROCESS = Process.PR_GM_IANC_BD_LOOP;
            Audits.ToCheck = Database.PR_GM_IANC_BDs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_GM_IANC_BD audit loop is starting");

            // Loop through the records
            foreach (var pr_gm_ianc_bd in Database.PR_GM_IANC_BDs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result10 = AuditStrategies.AuditA10(pr_gm_ianc_bd);
                if (!result10.Passed)
                {
                    HandleAuditFailure("AUDIT-A10", result10.Message, result10.FixScript, result10.RequiresManualFix, AuditCategory.Announcement);
                }
            }

            // Set values for the PR_EXT loop
            Globals.PROCESS = Process.PR_EXT_LOOP;
            Audits.ToCheck = Database.PR_EXTs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_EXT audit loop is starting");

            // Loop through the records
            foreach (var pr_ext in Database.PR_EXTs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result04 = AuditStrategies.AuditA04(pr_ext);
                if (!result04.Passed)
                {
                    HandleAuditFailure("AUDIT-A04", result04.Message, result04.FixScript, result04.RequiresManualFix, AuditCategory.Announcement);
                }
            }

            // Set values for the PR_UDATA loop
            Globals.PROCESS = Process.PR_UDATA_LOOP;
            Audits.ToCheck = Database.PR_UDATAs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_UDATA audit loop is starting");

            // Loop through the records
            foreach (var pr_udata in Database.PR_UDATAs)
            {
                // Pre-checks
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // Run the audits
                var result07 = AuditStrategies.AuditA07(pr_udata);
                if (!result07.Passed)
                {
                    HandleAuditFailure("AUDIT-A07", result07.Message, result07.FixScript, result07.RequiresManualFix, AuditCategory.Announcement);
                }
            }

            // Set values for the PR_UDATA loop
            Globals.PROCESS = Process.PR_AUDIO_GRP_LOOP;
            Audits.ToCheck = Database.PR_AUDIO_GRPs.Count;
            Audits.Checked = 0;
            Globals.GUI.AddStatus("PR_AUDIO_GRP audit loop is starting");

            // COMPLICATED AUDIT LOOP - PLEASE BE CAREFUL WITH CHANGES
            foreach (var pr_audio_grp in Database.PR_AUDIO_GRPs)
            {
                Audits.Checked++;
                if (Globals.CANCEL) return;

                // We need the UID of an announcement using this group, lets find one
                var pr_int_annc = Database.PR_INT_ANNCs.Find(a => a.AudioGroup == pr_audio_grp.AudioGroup);

                // Make sure we have one
                if (pr_int_annc == null) continue;

                // Now, get all the boards that use this UID
                var pr_gm_ianc_bds = Database.PR_GM_IANC_BDs.FindAll(a => a.UID == pr_int_annc.UID);

                // Loop through these boards
                foreach (var pr_gm_ianc_bd in pr_gm_ianc_bds)
                {
                    var result11 = AuditStrategies.AuditA11(pr_gm_ianc_bd, pr_audio_grp.AudioGroup);
                    if (!result11.Passed)
                    {
                        HandleAuditFailure("AUDIT-A11", result11.Message, result11.FixScript, result11.RequiresManualFix, AuditCategory.Announcement);
                    }
                }
            }
        }
    }
}