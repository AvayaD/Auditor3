/*
 * Auditor3 :: Audits
 * 
 * This class defines the audits that are run against the database objects.
 * 
 * Python Auditor3 is developed and maintained by David McNutt mcnuttd@avaya.com
 * 
 */

using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Auditor3 {
    internal static class Audits {

        internal static int Corrupted;              // Count of corrupted records
        internal static int CorruptedStations;      // Count of corrupted station records
        internal static int CorruptedTrunks;        // Count of corrupted trunk records
        internal static int CorruptedAnnouncements; // Count of corrupted announcement records
        internal static int ManualFixes;            // Count of required manual fixes

        internal static int Checked;                // Count of records that have been checked
        internal static int ToCheck;                // Count of records that need to be checked

        internal static int AuditS01Hits;           // Count of AUDITS01 issues
        internal static int AuditS02Hits;           // Count of AUDITS02 issues
        internal static int AuditS03Hits;           // Count of AUDITS03 issues
        internal static int AuditS04Hits;           // Count of AUDITS04 issues
        internal static int AuditS05Hits;           // Count of AUDITS05 issues
        internal static int AuditS06Hits;           // Count of AUDITS06 issues
        internal static int AuditS07Hits;           // Count of AUDITS07 issues
        internal static int AuditS08Hits;           // Count of AUDITS08 issues
        internal static int AuditS09Hits;           // Count of AUDITS09 issues
        internal static int AuditS10Hits;           // Count of AUDITS10 issues
        internal static int AuditS11Hits;           // Count of AUDITS11 issues
        internal static int AuditS12Hits;           // Count of AUDITS12 issues
        internal static int AuditS13Hits;           // Count of AUDITS13 issues
        internal static int AuditS14Hits;           // Count of AUDITS14 issues
        internal static int AuditS15Hits;           // Count of AUDITS15 issues
        internal static int AuditS16Hits;           // Count of AUDITS16 issues
        internal static int AuditS17Hits;           // Count of AUDITS17 issues
        internal static int AuditS18Hits;           // Count of AUDITS18 issues
        internal static int AuditS19Hits;           // Count of AUDITS19 issues
        internal static int AuditS20Hits;           // Count of AUDITS20 issues
        internal static int AuditS21Hits;           // Count of AUDITS21 issues
        internal static int AuditS22Hits;           // Count of AUDITS22 issues
        internal static int AuditS23Hits;           // Count of AUDITS23 issues
        internal static int AuditS24Hits;           // Count of AUDITS24 issues
        internal static int AuditS25Hits;           // Count of AUDITS25 issues
        internal static int AuditS26Hits;           // Count of AUDITS26 issues
        internal static int AuditS27Hits;           // Count of AUDITS27 issues
        internal static int AuditS28Hits;           // Count of AUDITS28 issues
        internal static int AuditS29Hits;           // Count of AUDITS29 issues
        internal static int AuditS30Hits;           // Count of AUDITS30 issues
        internal static int AuditS31Hits;           // Count of AUDITS31 issues
        internal static int AuditS32Hits;           // Count of AUDITS32 issues
        internal static int AuditS33Hits;           // Count of AUDITS33 issues
        internal static int AuditS34Hits;           // Count of AUDITS34 issues
        internal static int AuditS35Hits;           // Count of AUDITS35 issues

        internal static int AuditA01Hits;           // Count of AUDITA01 issues
        internal static int AuditA02Hits;           // Count of AUDITA02 issues
        internal static int AuditA03Hits;           // Count of AUDITA03 issues
        internal static int AuditA04Hits;           // Count of AUDITA04 issues
        internal static int AuditA05Hits;           // Count of AUDITA05 issues
        internal static int AuditA06Hits;           // Count of AUDITA06 issues
        internal static int AuditA07Hits;           // Count of AUDITA07 issues
        internal static int AuditA08Hits;           // Count of AUDITA08 issues
        internal static int AuditA09Hits;           // Count of AUDITA09 issues
        internal static int AuditA10Hits;           // Count of AUDITA10 issues
        internal static int AuditA11Hits;           // Count of AUDITA11 issues

        internal static int AuditT01Hits;           // Count of AUDITT01 issues
        internal static int AuditT02Hits;           // Count of AUDITT02 issues
        internal static int AuditT03Hits;           // Count of AUDITT03 issues
        internal static int AuditT04Hits;           // Count of AUDITT04 issues
        internal static int AuditT05Hits;           // Count of AUDITT05 issues
        internal static int AuditT06Hits;           // Count of AUDITT06 issues
        internal static int AuditT07Hits;           // Count of AUDITT07 issues
        internal static int AuditT08Hits;           // Count of AUDITT08 issues
        internal static int AuditT09Hits;

        // This method resets all the counters to zero
        internal static void ResetCounters() {
            Corrupted = 0;
            CorruptedStations = 0;
            CorruptedTrunks = 0;
            CorruptedAnnouncements = 0;
            ManualFixes = 0;

            AuditS01Hits = 0;
            AuditS02Hits = 0;
            AuditS03Hits = 0;
            AuditS04Hits = 0;
            AuditS05Hits = 0;
            AuditS06Hits = 0;
            AuditS07Hits = 0;
            AuditS08Hits = 0;
            AuditS09Hits = 0;
            AuditS10Hits = 0;
            AuditS11Hits = 0;
            AuditS12Hits = 0;
            AuditS13Hits = 0;
            AuditS14Hits = 0;
            AuditS15Hits = 0;
            AuditS16Hits = 0;
            AuditS17Hits = 0;
            AuditS18Hits = 0;
            AuditS19Hits = 0;
            AuditS20Hits = 0;
            AuditS21Hits = 0;
            AuditS22Hits = 0;
            AuditS23Hits = 0;
            AuditS24Hits = 0;
            AuditS25Hits = 0;
            AuditS26Hits = 0;
            AuditS27Hits = 0;
            AuditS28Hits = 0;
            AuditS29Hits = 0;
            AuditS30Hits = 0;
            AuditS31Hits = 0;
            AuditS32Hits = 0;
            AuditS33Hits = 0;
            AuditS34Hits = 0;
            AuditS35Hits = 0;

            AuditA01Hits = 0;
            AuditA02Hits = 0;
            AuditA03Hits = 0;
            AuditA04Hits = 0;
            AuditA05Hits = 0;
            AuditA06Hits = 0;
            AuditA07Hits = 0;
            AuditA08Hits = 0;
            AuditA09Hits = 0;
            AuditA10Hits = 0;
            AuditA11Hits = 0;

            AuditT01Hits = 0;
            AuditT02Hits = 0;
            AuditT03Hits = 0;
            AuditT04Hits = 0;
            AuditT05Hits = 0;
            AuditT06Hits = 0;
            AuditT07Hits = 0;
            AuditT08Hits = 0;
            AuditT09Hits = 0;
        }

        // This method is used to show the counter totals
        internal static void ShowCounters() {
            // Create a string builder to store the counts
            var counts = new StringBuilder();

            // Add the relevant counts
            counts.AppendLine($"CORRUPTED               : {Corrupted}");
            if (Globals.STATION_AUDITS)
                counts.AppendLine($"CORRUPTED STATIONS      : {CorruptedStations}");
            if (Globals.TRUNK_AUDITS)
                counts.AppendLine($"CORRUPTED TRUNKS        : {CorruptedTrunks}");
            if (Globals.ANNOUNCEMENT_AUDITS)
                counts.AppendLine($"CORRUPTED ANNOUNCEMENTS : {CorruptedAnnouncements}");
            counts.AppendLine($"MANUAL FIXES            : {ManualFixes}");
            counts.AppendLine();
            if (AuditS01Hits > 0)
                counts.AppendLine($"AUDIT-S01 : {AuditS01Hits}");
            if (AuditS02Hits > 0)
                counts.AppendLine($"AUDIT-S02 : {AuditS02Hits}");
            if (AuditS03Hits > 0)
                counts.AppendLine($"AUDIT-S03 : {AuditS03Hits}");
            if (AuditS04Hits > 0)
                counts.AppendLine($"AUDIT-S04 : {AuditS04Hits}");
            if (AuditS05Hits > 0)
                counts.AppendLine($"AUDIT-S05 : {AuditS05Hits}");
            if (AuditS06Hits > 0)
                counts.AppendLine($"AUDIT-S06 : {AuditS06Hits}");
            if (AuditS07Hits > 0)
                counts.AppendLine($"AUDIT-S07 : {AuditS07Hits}");
            if (AuditS08Hits > 0)
                counts.AppendLine($"AUDIT-S08 : {AuditS08Hits}");
            if (AuditS09Hits > 0)
                counts.AppendLine($"AUDIT-S09 : {AuditS09Hits}");
            if (AuditS10Hits > 0)
                counts.AppendLine($"AUDIT-S10 : {AuditS10Hits}");
            if (AuditS11Hits > 0)
                counts.AppendLine($"AUDIT-S11 : {AuditS11Hits}");
            if (AuditS12Hits > 0)
                counts.AppendLine($"AUDIT-S12 : {AuditS12Hits}");
            if (AuditS13Hits > 0)
                counts.AppendLine($"AUDIT-S13 : {AuditS13Hits}");
            if (AuditS14Hits > 0)
                counts.AppendLine($"AUDIT-S14 : {AuditS14Hits}");
            if (AuditS15Hits > 0)
                counts.AppendLine($"AUDIT-S15 : {AuditS15Hits}");
            if (AuditS16Hits > 0)
                counts.AppendLine($"AUDIT-S16 : {AuditS16Hits}");
            if (AuditS17Hits > 0)
                counts.AppendLine($"AUDIT-S17 : {AuditS17Hits}");
            if (AuditS18Hits > 0)
                counts.AppendLine($"AUDIT-S18 : {AuditS18Hits}");
            if (AuditS19Hits > 0)
                counts.AppendLine($"AUDIT-S19 : {AuditS19Hits}");
            if (AuditS20Hits > 0)
                counts.AppendLine($"AUDIT-S20 : {AuditS20Hits}");
            if (AuditS21Hits > 0)
                counts.AppendLine($"AUDIT-S21 : {AuditS21Hits}");
            if (AuditS22Hits > 0)
                counts.AppendLine($"AUDIT-S22 : {AuditS22Hits}");
            if (AuditS23Hits > 0)
                counts.AppendLine($"AUDIT-S23 : {AuditS23Hits}");
            if (AuditS24Hits > 0)
                counts.AppendLine($"AUDIT-S24 : {AuditS24Hits}");
            if (AuditS25Hits > 0)
                counts.AppendLine($"AUDIT-S25 : {AuditS25Hits}");
            if (AuditS26Hits > 0)
                counts.AppendLine($"AUDIT-S26 : {AuditS26Hits}");
            if (AuditS27Hits > 0)
                counts.AppendLine($"AUDIT-S27 : {AuditS27Hits}");
            if (AuditS28Hits > 0)
                counts.AppendLine($"AUDIT-S28 : {AuditS28Hits}");
            if (AuditS29Hits > 0)
                counts.AppendLine($"AUDIT-S29 : {AuditS29Hits}");
            if (AuditS30Hits > 0)
                counts.AppendLine($"AUDIT-S30 : {AuditS30Hits}");
            if (AuditS31Hits > 0)
                counts.AppendLine($"AUDIT-S31 : {AuditS31Hits}");
            if (AuditS32Hits > 0)
                counts.AppendLine($"AUDIT-S32 : {AuditS32Hits}");
            if (AuditS33Hits > 0)
                counts.AppendLine($"AUDIT-S33 : {AuditS33Hits}");
            if (AuditS34Hits > 0)
                counts.AppendLine($"AUDIT-S34 : {AuditS34Hits}");
            if (AuditS35Hits > 0)
                counts.AppendLine($"AUDIT-S35 : {AuditS35Hits}");

            if (AuditA01Hits > 0)
                counts.AppendLine($"AUDIT-A01 : {AuditA01Hits}");
            if (AuditA02Hits > 0)
                counts.AppendLine($"AUDIT-A02 : {AuditA02Hits}");
            if (AuditA03Hits > 0)
                counts.AppendLine($"AUDIT-A03 : {AuditA03Hits}");
            if (AuditA04Hits > 0)
                counts.AppendLine($"AUDIT-A04 : {AuditA04Hits}");
            if (AuditA05Hits > 0)
                counts.AppendLine($"AUDIT-A05 : {AuditA05Hits}");
            if (AuditA06Hits > 0)
                counts.AppendLine($"AUDIT-A06 : {AuditA06Hits}");
            if (AuditA07Hits > 0)
                counts.AppendLine($"AUDIT-A07 : {AuditA07Hits}");
            if (AuditA08Hits > 0)
                counts.AppendLine($"AUDIT-A08 : {AuditA08Hits}");
            if (AuditA09Hits > 0)
                counts.AppendLine($"AUDIT-A09 : {AuditA09Hits}");
            if (AuditA10Hits > 0)
                counts.AppendLine($"AUDIT-A10 : {AuditA10Hits}");
            if (AuditA11Hits > 0)
                counts.AppendLine($"AUDIT-A11 : {AuditA11Hits}");

            if (AuditT01Hits > 0)
                counts.AppendLine($"AUDIT-T01 : {AuditT01Hits}");
            if (AuditT02Hits > 0)
                counts.AppendLine($"AUDIT-T02 : {AuditT02Hits}");
            if (AuditT03Hits > 0)
                counts.AppendLine($"AUDIT-T03 : {AuditT03Hits}");
            if (AuditT04Hits > 0)
                counts.AppendLine($"AUDIT-T04 : {AuditT04Hits}");
            if (AuditT05Hits > 0)
                counts.AppendLine($"AUDIT-T05 : {AuditT05Hits}");
            if (AuditT06Hits > 0)
                counts.AppendLine($"AUDIT-T06 : {AuditT06Hits}");
            if (AuditT07Hits > 0)
                counts.AppendLine($"AUDIT-T07 : {AuditT07Hits}");
            if (AuditT08Hits > 0)
                counts.AppendLine($"AUDIT-T08 : {AuditT08Hits}");
            if(AuditT09Hits > 0)
                counts.AppendLine($"AUDIT-T09 : {AuditT09Hits}");

            // Output the counts to the report
            Globals.GUI.AddOutput(counts.ToString());
        }

        // AUDIT-S01 : This audit will check a PR_STN record and ensure it has a PR_UDATA
        internal static bool AuditS01(PR_STN pr_stn) {
            if (!pr_stn.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S01");
                message.AppendLine("PR_STN is missing PR_UDATA");
                message.AppendLine($"UID: {pr_stn.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.Station(pr_stn.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS01Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S02 : This will check a PR_ST_CPS record and ensure it has both a PR_PORT_UID
        // and a PR_MOPORT. If it fails, it has to be the only UID using the port, otherwise
        // it will trigger AUDITS03 instead
        internal static bool AuditS02(PR_ST_CPS pr_st_cps) {
            if (!pr_st_cps.HasPORTUID() && !pr_st_cps.HasMOPORT() && pr_st_cps.HasSTN() &&
                !pr_st_cps.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S02");
                message.AppendLine("PR_ST_CPS is missing PR_PORT_UID and PR_MOPORT");
                message.AppendLine($"UID: {pr_st_cps.UID}");
                message.AppendLine($"Port: {pr_st_cps.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_MOPORT(pr_st_cps.UID, pr_st_cps.Port));
                message.AppendLine(Fixer.Add.PR_PORT_UID(pr_st_cps.UID, pr_st_cps.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS02Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S03 : This will check a PR_ST_CPS record and ensure it has both a PR_PORT_UID
        // and a PR_MOPORT. If it fails, this will trigger if there are multiple UIDs using the
        // port, otherwise it would have already triggered AUDITS02 instead
        internal static bool AuditS03(PR_ST_CPS pr_st_cps) {
            if (!pr_st_cps.HasPORTUID() && !pr_st_cps.HasMOPORT() && pr_st_cps.HasSTN() &&
                pr_st_cps.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S03");
                message.AppendLine("Incorrect port assigned in PR_ST_CPS");
                message.AppendLine($"UID: {pr_st_cps.UID}");
                message.AppendLine($"Port: {pr_st_cps.Port}");
                message.AppendLine();

                // If this is an IP station, we can assign an unused port, otherwise it needs to
                // be manually fixed, likely making it an x-port
                var pr_stn = Database.PR_STNs.Find(a => a.UID == pr_st_cps.UID);
                if (pr_stn.IsIP()) {
                    message.AppendLine(Fixer.Add.NewIPPort(pr_st_cps.UID));
                } else {
                    message.AppendLine("** UNABLE TO ASSIGN TDM PORT AUTOMATICALLY **");
                    message.AppendLine("** MANUAL FIX REQUIRED **");
                    ManualFixes++;
                    Database.ManualPorts.Add(pr_st_cps.Port);
                    Database.ManualUIDs.Add(pr_st_cps.UID);
                }

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS03Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S04 : This audit will check a PR_STN record and ensure it has 
        // a PR_EXT on the same UID. This audit does not run against ATTD_USER GID
        internal static bool AuditS04(PR_STN pr_stn) {
            if (pr_stn.GID != "0002" && !pr_stn.HasEXT()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S04");
                message.AppendLine("PR_STN is missing PR_EXT");
                message.AppendLine($"UID: {pr_stn.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.Station(pr_stn.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS04Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S05 : This audit will check a PR_EXT record and ensure it has
        // a PR_UDATA with the same UID. This only runs for station extensions
        internal static bool AuditS05(PR_EXT pr_ext) {
            if (pr_ext.GID == "0000" && !pr_ext.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S05");
                message.AppendLine("PR_EXT has no PR_UDATA");
                message.AppendLine($"UID: {pr_ext.UID}");
                message.AppendLine($"Extension: {pr_ext.Digits}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_EXT(pr_ext.Digits));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS05Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S06 : This audit will check if a PR_PORT_UID is missing both
        // PR_MOPORT and PR_ST_CPS
        internal static bool AuditS06(PR_PORT_UID pr_port_uid) {
            if (!pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S06");
                message.AppendLine("PR_PORT_UID has neither PR_MOPORT nor PR_ST_CPS");
                message.AppendLine($"UID: {pr_port_uid.UID}");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_PORT_UID(pr_port_uid.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS06Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S07 : This audit will check a PR_PORT_UID for missing PR_MOPORT
        // but does have a PR_ST_CPS, and no duplicates PR_ST_CPS
        internal static bool AuditS07(PR_PORT_UID pr_port_uid) {
            if (!pr_port_uid.HasMOPORT() && pr_port_uid.HasSTCPS() && !pr_port_uid.HasDuplicateSTCPS()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S07");
                message.AppendLine("PR_PORT_UID is missing PR_MOPORT");
                message.AppendLine($"UID: {pr_port_uid.UID}");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_MOPORT(pr_port_uid.UID, pr_port_uid.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS07Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S08 : This audit will check a PR_PORT_UID for missing PR_ST_CPS
        // but does have a PR_MOPORT and a valid station owns the port
        internal static bool AuditS08(PR_PORT_UID pr_port_uid) {
            if (pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS() && pr_port_uid.UID != Globals.NULL_UID &&
                pr_port_uid.GID != "0034" && !pr_port_uid.UIDHasDuplicatePort() && !pr_port_uid.UIDOwnsAnotherPort() &&
                pr_port_uid.HasSTN()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S08");
                message.AppendLine("PR_PORT_UID is missing PR_ST_CPS");
                message.AppendLine($"UID: {pr_port_uid.UID}");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_ST_CPS(pr_port_uid.UID, pr_port_uid.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS08Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S09 : DUPLICATE PORT ISSUE AUDIT
        internal static bool AuditS09(PR_PORT_UID pr_port_uid) {
            var success = true;
            var ports = Database.PR_ST_CPSs.FindAll(a => a.Port == pr_port_uid.Port);
            foreach (var port in ports.Where(a => a.UID != pr_port_uid.UID && a.HasSTN())) {
                success = false;
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S09");
                message.AppendLine("Incorrect PR_ST_CPS");
                message.AppendLine($"UID: {port.UID}");
                message.AppendLine($"Port: {port.Port}");

                if (ports.Count > 1) {
                    message.AppendLine("** DUPLICATE PORT ISSUE **");
                    foreach (var cps in ports) {
                        message.AppendLine($"UID: {cps.UID}");
                    }
                }

                message.AppendLine();
                message.AppendLine(Fixer.Update.PR_ST_CPS(port.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS09Hits++;
            }
            return success;
        }

        // AUDIT-S10 : This audit checks every IP station's PR_ST_CPS to ensure it has
        // a valid 7f port
        internal static bool AuditS10(PR_STN pr_stn) {
            if (pr_stn.IsIP() && !pr_stn.HasValidIPPort()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S10");
                message.AppendLine("IP station does not have valid 7fxxxxxx port");
                message.AppendLine($"UID: {pr_stn.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.NewIPPort(pr_stn.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS10Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S11 : This audit checked every PR_BUTTON record to ensure it has
        // a valid PR_STN or PR_UDATA record
        internal static bool AuditS11(PR_BUTTON pr_button) {
            if (pr_button.Number == "0001" && !pr_button.HasSTN() && !pr_button.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S11");
                message.AppendLine("PR_BUTTON has neither PR_STN nor PR_UDATA");
                message.AppendLine($"UID: {pr_button.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_BUTTON(pr_button.UID, "0001"));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS11Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S12 : This audit triggers when a PR_ST_CPS record is missing a PR_PORT_UID
        // but does have a PR_MOPORT
        internal static bool AuditS12(PR_ST_CPS pr_st_cps) {
            if (!pr_st_cps.HasPORTUID() && pr_st_cps.HasMOPORT() && !pr_st_cps.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S12");
                message.AppendLine("PR_ST_CPS is missing PR_PORT_UID");
                message.AppendLine($"UID: {pr_st_cps.UID}");
                message.AppendLine($"Port: {pr_st_cps.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_PORT_UID(pr_st_cps.UID, pr_st_cps.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS12Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S13 : This audit triggers when a PR_MOPORT is missing PR_PORT_UID but
        // does have a PR_ST_CPS
        internal static bool AuditS13(PR_MOPORT pr_moport) {
            if (pr_moport.HasSTCPS() && !pr_moport.HasPORTUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S13");
                message.AppendLine("PR_MOPORT is missing PR_PORT_UID");
                message.AppendLine($"UID: {pr_moport.UID()}");
                message.AppendLine($"Port: {pr_moport.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_PORT_UID(pr_moport.UID(), pr_moport.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS13Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S14 : This audit triggers when a PR_MOPORT has neither PR_ST_CPS nor
        // PR_PORT_UID and is basically an abandoned record
        internal static bool AuditS14(PR_MOPORT pr_moport) {
            if (!pr_moport.HasSTCPS() && !pr_moport.HasPORTUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S14");
                message.AppendLine("PR_MOPORT is abandoned");
                message.AppendLine($"UID: {pr_moport.UID()}");
                message.AppendLine($"Port: {pr_moport.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_MOPORT(pr_moport.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS14Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S15 : This audit triggers when the UID is null for a PR_PORT_UID
        // and we can find a valid owner
        internal static bool AuditS15(PR_PORT_UID pr_port_uid) {
            if (pr_port_uid.HasMOPORT() && pr_port_uid.UID == Globals.NULL_UID && 
                pr_port_uid.ValidOwner() != null) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S15");
                message.AppendLine("PR_PORT_UID has incorrect UID");
                message.AppendLine($"Old UID: {pr_port_uid.UID}");
                message.AppendLine($"New UID: {pr_port_uid.ValidOwner()}");
                message.AppendLine();
                message.AppendLine(Fixer.Update.PR_PORT_UID(pr_port_uid.Port, pr_port_uid.ValidOwner()));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS15Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S16 : This audit triggers when the UID is null for a PR_PORT_UID
        // and we cannot find a valid owner
        internal static bool AuditS16(PR_PORT_UID pr_port_uid) {
            if (pr_port_uid.HasMOPORT() && pr_port_uid.UID == Globals.NULL_UID && 
                pr_port_uid.ValidOwner() == null) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S16");
                message.AppendLine("PR_PORT_UID and PR_MOPORT are abandoned");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_PORT_UID(pr_port_uid.Port));
                message.AppendLine(Fixer.Remove.PR_MOPORT(pr_port_uid.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS16Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S17 : This audit triggers when a PR_PORT_UID does not have a PR_ST_CPS,
        // does have a PR_MOPORT, but the UID owns another port, so this one is orphaned
        internal static bool AuditS17(PR_PORT_UID pr_port_uid) {
            if (pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS() && pr_port_uid.UID != Globals.NULL_UID &&
                pr_port_uid.GID != "0034" && !pr_port_uid.UIDOwnsAnotherPort() && !pr_port_uid.UIDHasDuplicatePort()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S17");
                message.AppendLine("PR_PORT_UID and PR_MOPORT are abandoned");
                message.AppendLine($"UID: {pr_port_uid.UID}");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_MOPORT(pr_port_uid.Port));
                message.AppendLine(Fixer.Remove.PR_PORT_UID(pr_port_uid.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS17Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S18 : This audit triggers when a bridged appearance PR_BUTTON does not
        // point to a valid UID
        internal static bool AuditS18(PR_BUTTON pr_button) {
            if (pr_button.Bridged && !pr_button.HasValidBridgedUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S18");
                message.AppendLine("Bridged PR_BUTTON has invalid target UID");
                message.AppendLine($"UID: {pr_button.UID}");
                message.AppendLine($"Number: {pr_button.Number}");
                message.AppendLine($"Target UID: {pr_button.BridgedUID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_BUTTON(pr_button.UID, pr_button.Number));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS18Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S19 : This audit triggers when a PR_BRIDGE record does not have a valid
        // primary UID
        internal static bool AuditS19(PR_BRIDGE pr_bridge) {
            if (!pr_bridge.HasValidPrimaryUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S19");
                message.AppendLine("PR_BRIDGE has invalid primary UID");
                message.AppendLine($"Primary UID: {pr_bridge.PrimaryUID}");
                message.AppendLine($"Bridged UID: {pr_bridge.BridgedUID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_BRIDGE(pr_bridge));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS19Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S20 : This audit triggers when a PR_ST_CPS record has neither a
        // PR_STN or PR_UDATA
        internal static bool AuditS20(PR_ST_CPS pr_st_cps) {
            if (!pr_st_cps.HasSTN() && !pr_st_cps.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S20");
                message.AppendLine("PR_ST_CPS is missing PR_STN and PR_UDATA");
                message.AppendLine($"UID: {pr_st_cps.UID}");
                message.AppendLine($"Port: {pr_st_cps.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_ST_CPS(pr_st_cps.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS20Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S21 : This audit triggers when a PR_PORT_UID does not
        // have a valid station
        internal static bool AuditS21(PR_PORT_UID pr_port_uid) {
            if (pr_port_uid.HasMOPORT() && !pr_port_uid.HasSTCPS() && pr_port_uid.UID != Globals.NULL_UID &&
                pr_port_uid.GID != "0034" && !pr_port_uid.UIDHasDuplicatePort() && !pr_port_uid.UIDOwnsAnotherPort()
                && !pr_port_uid.HasSTN()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S21");
                message.AppendLine("Port is abandoned");
                message.AppendLine($"UID: {pr_port_uid.UID}");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_PORT_UID(pr_port_uid.Port));
                message.AppendLine(Fixer.Remove.PR_MOPORT(pr_port_uid.Port));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS21Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S22 : This audit triggers when a PR_UDATA does not have a PR_STN
        internal static bool AuditS22(PR_UDATA pr_udata) {
            if (!pr_udata.HasSTN()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S22");
                message.AppendLine("PR_UDATA does not have PR_STN");
                message.AppendLine($"UID: {pr_udata.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.Station(pr_udata.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS22Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S23 : This audit triggers when a PR_XMAP does not have a PR_OPT_STN
        internal static bool AuditS23(PR_XMAP pr_xmap) {
            if (!pr_xmap.HasOPTSTN() && !pr_xmap.IsXMOBILE()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S23");
                message.AppendLine("PR_XMAP does not have PR_OPT_STN");
                message.AppendLine($"UID: {pr_xmap.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_XMAP(pr_xmap));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS23Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S24 : This audit triggers when a PR_OPT_STN does not have a PR_XMAP
        internal static bool AuditS24(PR_OPT_STN pr_opt_stn) {
            if (!pr_opt_stn.HasXMAP()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S24");
                message.AppendLine("PR_OPT_STN does not have PR_XMAP");
                message.AppendLine($"UID: {pr_opt_stn.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_OPT_STN(pr_opt_stn));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS24Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S25 : This audit triggers when a PR_XMAP does not have a PR_UDATA
        internal static bool AuditS25(PR_XMAP pr_xmap) {
            if (!pr_xmap.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S25");
                message.AppendLine("PR_XMAP does not have PR_UDATA");
                message.AppendLine($"UID: {pr_xmap.UID}");
                message.AppendLine();
                message.AppendLine("- You will need to add a fake PR_UDATA to remove this record");
                message.AppendLine(string.Join(Environment.NewLine, pr_xmap.PREC));
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Database.ManualUIDs.Add(pr_xmap.UID);
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                ManualFixes++;
                AuditS25Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S26 : This audit triggers when a PR_OPT_STN does not have a PR_UDATA
        internal static bool AuditS26(PR_OPT_STN pr_opt_stn) {
            if (!pr_opt_stn.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S26");
                message.AppendLine("PR_OPT_STN does not have PR_UDATA");
                message.AppendLine($"UID: {pr_opt_stn.UID}");
                message.AppendLine();
                message.AppendLine("- You will need to add a fake PR_UDATA to remove this record");
                message.AppendLine(string.Join(Environment.NewLine, pr_opt_stn.PREC));
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Database.ManualUIDs.Add(pr_opt_stn.UID);
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                ManualFixes++;
                AuditS26Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S27 : This audit triggers when a PR_STN does not have a PR_FEXT
        internal static bool AuditS27(PR_STN pr_stn) {
            if (!pr_stn.HasFEXT() && pr_stn.HasEXT()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S27");
                message.AppendLine("PR_STN does not have PR_FEXT");
                message.AppendLine($"UID: {pr_stn.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_FEXT(pr_stn.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS27Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S28 : This audit triggers when a PR_STN has mismatched digits in PR_EXT/PR_FEXT
        internal static bool AuditS28(PR_STN pr_stn) {
            if (pr_stn.HasEXT() && pr_stn.HasUDATA() && !pr_stn.HasMatchingDigits()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S28");
                message.AppendLine("PR_STN has mismatched digits in PR_EXT and PR_FEXT");
                message.AppendLine($"UID: {pr_stn.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Update.PR_FEXT(pr_stn.UID));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS28Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S29 : This audit triggers when there is an AWOH mismatch
        internal static bool AuditS29(PR_STN pr_stn) {
            if (pr_stn.HasAWOHMismatch()) {
                // If this is an IP station and we are not AWOH, then we can skip, another
                // audit will add a proper IP port
                if (pr_stn.IsIP() && !pr_stn.AWOH) return true;
                var pr_st_cps = Database.PR_ST_CPSs.Find(a => a.UID == pr_stn.UID);
                var port = pr_st_cps != null ? pr_st_cps.Port : Globals.NULL_PORT;
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S29");
                message.AppendLine("AWOH MISMATCH");
                message.AppendLine($"UID : {pr_stn.UID}");
                message.AppendLine($"PORT: {port}");
                message.AppendLine($"AWOH: {pr_stn.AWOH}");
                message.AppendLine();
                
                if (pr_stn.IsIP() && pr_stn.AWOH) {
                    message.AppendLine("** IP STATION CANNOT BE AWOH **");
                    message.AppendLine("** MUST USE TCM BUFFER TO CHANGE more_info TO NOT BE AWOH **");
                    message.AppendLine("** MANUAL FIX REQUIRED **");
                    ManualFixes++;
                    if (port != Globals.NULL_PORT) Database.ManualPorts.Add(port);
                    Database.ManualUIDs.Add(pr_stn.UID);
                } else if (!pr_stn.IsIP() && pr_stn.AWOH && pr_stn.HasValidIPPort()) {
                    message.AppendLine("AWOH TDM station has IP port");
                    message.AppendLine();
                    message.AppendLine(Fixer.Remove.PR_MOPORT(port));
                    message.AppendLine(Fixer.Remove.PR_PORT_UID(port));
                    message.AppendLine(Fixer.Update.PR_ST_CPS(pr_stn.UID, Globals.NULL_PORT));
                } else if (!pr_stn.IsIP() && pr_stn.AWOH && port != Globals.NULL_PORT) {
                    message.AppendLine("** TDM STATION WITH PORT **");
                    message.AppendLine("** MUST USE TCM BUFFER TO CHANGE more_info TO NOT BE AWOH **");
                    message.AppendLine("** MANUAL FIX REQUIRED **");
                    ManualFixes++;
                    if (port != Globals.NULL_PORT) Database.ManualPorts.Add(port);
                    Database.ManualUIDs.Add(pr_stn.UID);
                } else if (!pr_stn.IsIP() && !pr_stn.AWOH && port == Globals.NULL_PORT) {
                    message.AppendLine("** TDM STATION WITHOUT PORT **");
                    message.AppendLine("** MUST USE TCM BUFFER TO CHANGE more_info TO BE AWOH **");
                    message.AppendLine("** MANUAL FIX REQUIRED **");
                    ManualFixes++;
                    if (port != Globals.NULL_PORT) Database.ManualPorts.Add(port);
                    Database.ManualUIDs.Add(pr_stn.UID);
                } else {
                    message.AppendLine("!!! UNKNOWN SITUATION !!!");
                    message.AppendLine("Please notify David McNutt to evaluate a new scenario");
                    message.AppendLine("Please include a copy of the customer XLN and swversion");
                }

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS29Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S30 : This audit triggers when there is a missing PR_MOBD
        internal static bool AuditS30(PR_ST_CPS pr_st_cps) {
            if (!pr_st_cps.IsIPPort() && pr_st_cps.Port != Globals.NULL_PORT && !pr_st_cps.HasMOBD()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S30");
                message.AppendLine("Missing PR_MOBD");
                message.AppendLine($"UID: {pr_st_cps.UID}");
                message.AppendLine($"Board: {pr_st_cps.Port}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Database.ManualUIDs.Add(pr_st_cps.UID);
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                ManualFixes++;
                AuditS30Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S31: This audit is used to check for PR_AMW records that are out of order
        internal static bool AuditS31() {
            string prevExt = null;
            string prevUID = null;
            bool fail;

            var message = new StringBuilder();

            foreach (var pr_amw in Database.PR_AMWs) {
                fail = false;

                if (prevExt == null) {
                    prevExt = pr_amw.Extension;
                    prevUID = pr_amw.UID;
                }
                else {
                    if (pr_amw.Extension == prevExt) {
                        int check1 = string.Compare(prevUID, pr_amw.UID);
                        if (check1 > 0) {
                            fail = true;
                        }
                    } else {
                        int check2 = string.Compare(prevExt, pr_amw.Extension);
                        if (check2 > 0) {
                            fail = true;
                        }
                    }

                    if (fail) {
                        message.AppendLine("AUDIT-S31");
                        message.AppendLine("PR_AMW is out of order");
                        message.AppendLine($"UID: {pr_amw.ActualUID}");
                        message.AppendLine($"EXT: {pr_amw.Extension}");
                        message.AppendLine();
                        message.AppendLine("** MANUAL FIX REQUIRED **");
                        message.AppendLine();

                        Corrupted++;
                        CorruptedStations++;
                        ManualFixes++;
                        AuditS31Hits++;
                    }

                    prevExt = pr_amw.Extension;
                    prevUID = pr_amw.UID;
                }
            }

            if (AuditS31Hits > 0) Globals.GUI.AddOutput(message.ToString());
            return AuditS31Hits == 0;
        }

        // AUDIT-S32: This audit checks for duplicate PR_AMW records
        internal static bool AuditS32(PR_AMW pr_amw) {
            if (!pr_amw.DupFlagged && pr_amw.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S32");
                message.AppendLine("PR_AMW is duplicate");
                message.AppendLine($"UID: {pr_amw.ActualUID}");
                message.AppendLine($"EXT: {pr_amw.Extension}");
                message.AppendLine();

                var pr_amws = Database.PR_AMWs.FindAll(a => a.ActualUID == pr_amw.ActualUID && a.Extension == pr_amw.Extension);
                var removes = pr_amws.Count - 1;
                for (int i = 0; i < removes; i++) {
                    message.AppendLine(Fixer.Remove.PR_AMW(pr_amw));
                }

                message.AppendLine();

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS32Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-S33: This audit checks for mismatched PR_AMW records
        internal static bool AuditS33(PR_AMW pr_amw) {
            if (pr_amw.IsMismatched() || pr_amw.MwlExtMismatch()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S33");
                message.AppendLine("PR_AMW is mismatched");
                message.AppendLine($"UID: {pr_amw.ActualUID}");
                message.AppendLine($"EXT: {pr_amw.UnpackedExt}");

                if (pr_amw.IsMismatched()) {
                    message.AppendLine();
                    message.AppendLine(Fixer.Update.PR_AMW(pr_amw));
                    message.AppendLine();
                }

                if (pr_amw.MwlExtMismatch()) {
                    var pr_stn = Database.PR_STNs.Find(a => a.UID == pr_amw.ActualUID);

                    message.AppendLine();
                    message.AppendLine("** PR_STN has incorrect mwl_ext **");
                    message.AppendLine();
                    message.AppendLine($"Station Ext: {pr_stn.StationExt()}");
                    message.AppendLine($"MWL Ext    : {pr_stn.MWLExt}");
                    message.AppendLine();
                    message.AppendLine(Fixer.Update.PR_AMW_REMOVE(pr_amw));
                    message.AppendLine();
                    message.AppendLine("REMOVED PR_AMW::");
                    message.AppendLine($"STATION: {pr_stn.StationExt()}   MWL: {pr_amw.UnpackedExt}");
                    message.AppendLine();
                }

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS33Hits++;
                return false;
            }

            return true;
        }

        // AUDIT-S34: Check to ensure PR_FEXTs are not orphaned
        internal static bool AuditS34(PR_FEXT pr_fext) {
            if (pr_fext.IsStation && !pr_fext.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S34");
                message.AppendLine("PR_FEXT is orphaned");
                message.AppendLine($"UID: {pr_fext.UID}");
                message.AppendLine($"EXT: {pr_fext.Digits}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_FEXT(pr_fext));
                message.AppendLine();

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS34Hits++;
                return false;
            }

            return true;
        }

        internal static bool AuditS35(PR_FEXT pr_fext) {
            if (pr_fext.HasDuplicateUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-S35");
                message.AppendLine("PR_FEXT is duplicate");
                message.AppendLine($"UID: {pr_fext.UID}");
                message.AppendLine($"EXT: {pr_fext.Digits}");
                message.AppendLine();

                //var pr_ext = Database.PR_EXTs.Find(a => a.Digits == pr_fext.Digits);
                //var pr_fexts = Database.PR_FEXTs.FindAll(a => a.UID == pr_fext.UID);

                //foreach (var fext in pr_fexts) {
                //    if (fext.UID != pr_ext.UID) {
                //        message.AppendLine(Fixer.Remove.PR_FEXT(fext));
                //    }
                //}

                message.AppendLine();

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedStations++;
                AuditS35Hits++;
                ManualFixes++;
                return false;
            }

            return true;
        }

            // AUDIT-A01 : This audit triggers when PR_INT_ANNC is missing either PR_IANC_BD or
            // PR_GM_IANC_BD
            internal static bool AuditA01(PR_INT_ANNC pr_int_annc) {
            if (pr_int_annc.AudioGroup == "00" && (!pr_int_annc.HasIANCBD() || !pr_int_annc.HasGMIANCBD())) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A01");
                message.AppendLine("PR_INT_ANNC is missing board PREC");
                message.AppendLine($"UID: {pr_int_annc.UID}");
                message.AppendLine();

                if (pr_int_annc.HasIANCBD() && !pr_int_annc.HasGMIANCBD()) {
                    message.AppendLine(Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_int_annc.Board));
                } else if (!pr_int_annc.HasIANCBD() && pr_int_annc.HasGMIANCBD()) {
                    message.AppendLine(Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_int_annc.Board, pr_int_annc.IndexLName));
                } else {
                    message.AppendLine(Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_int_annc.Board));
                    message.AppendLine(Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_int_annc.Board, pr_int_annc.IndexLName));
                }

                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                AuditA01Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A02 : This audit checks all the board PRECs for an audio group
        internal static bool AuditA02(PR_INT_ANNC pr_int_annc) {
            if (pr_int_annc.AudioGroup == "00") return true;
            var success = true;
            var pr_ag_mbrs = Database.PR_AG_MBRs.FindAll(a => a.AudioGroup == pr_int_annc.AudioGroup);
            foreach (var pr_ag_mbr in pr_ag_mbrs) {
                var pr_ianc_bd = Database.PR_IANC_BDs.Find(a => a.UID == pr_int_annc.UID && a.Board == pr_ag_mbr.Board);
                var pr_gm_ianc_bd = Database.PR_GM_IANC_BDs.Find(a => a.UID == pr_int_annc.UID && a.Board == pr_ag_mbr.Board);
                if (pr_ianc_bd == null || pr_gm_ianc_bd == null) {
                    success = false;
                    var message = new StringBuilder();
                    message.AppendLine("AUDIT-A02");
                    message.AppendLine("Missing audio group board PRECs");
                    message.AppendLine($"UID: {pr_int_annc.UID}");
                    message.AppendLine();
                    if (pr_ianc_bd != null && pr_gm_ianc_bd == null) {
                        message.AppendLine(Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board));
                    } else if (pr_ianc_bd == null && pr_gm_ianc_bd != null) {
                        message.AppendLine(Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board, pr_int_annc.IndexLName));
                    } else {
                        message.AppendLine(Fixer.Add.PR_GM_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board));
                        message.AppendLine(Fixer.Add.PR_IANC_BD(pr_int_annc.UID, pr_ag_mbr.Board, pr_int_annc.IndexLName));
                    }
                    Globals.GUI.AddOutput(message.ToString());
                    Corrupted++;
                    CorruptedAnnouncements++;
                    AuditA02Hits++;
                }
            }
            return success;
        }

        // AUDIT-A03 : This audit triggers when there it a duplicate PR_IANC_BD
        internal static bool AuditA03(PR_IANC_BD pr_ianc_bd) {
            if (pr_ianc_bd.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A03");
                message.AppendLine("Duplicate PR_IANC_BD");
                message.AppendLine($"UID: {pr_ianc_bd.UID}");
                message.AppendLine($"Board: {pr_ianc_bd.Board}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA03Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A04 : This audit triggers if a PR_EXT with GID of 008c (ANNC)
        // does not have a PR_INT_ANNC record
        internal static bool AuditA04(PR_EXT pr_ext) {
            if (pr_ext.GID == "008c" && !pr_ext.HasINTANNC()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A04");
                message.AppendLine("Missing PR_INT_ANNC");
                message.AppendLine($"UID: {pr_ext.UID}");
                message.AppendLine($"EXT: {pr_ext.Digits}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA04Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A05 : This audit triggers if a PR_INT_ANNC does not have
        // a PR_EXT record
        internal static bool AuditA05(PR_INT_ANNC pr_int_annc) {
            if (!pr_int_annc.HasEXT()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A05");
                message.AppendLine("PR_INT_ANNC is missing PR_EXT");
                message.AppendLine($"UID: {pr_int_annc.UID}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA05Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A06 : This audit triggers if a PR_INT_ANNC does not have a PR_UDATA
        internal static bool AuditA06(PR_INT_ANNC pr_int_annc) {
            if (!pr_int_annc.HasUDATA()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A06");
                message.AppendLine("PR_INT_ANNC is missing PR_UDATA");
                message.AppendLine($"UID: {pr_int_annc.UID}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA06Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A07 : This audit triggers if a PR_UDATA record with GID 008c (ANNC) does
        // not have a PR_INT_ANNC
        internal static bool AuditA07(PR_UDATA pr_udata) {
            if (pr_udata.GID == "008c" && !pr_udata.HasINTANNC()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A07");
                message.AppendLine("PR_UDATA is missing PR_INT_ANNC");
                message.AppendLine($"UID: {pr_udata.UID}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA07Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A08 : This audit triggers when there is an AudioGroup mismatch
        // between PR_INT_ANNC and PR_AN_GRP
        internal static bool AuditA08(PR_INT_ANNC pr_int_annc) {
            if (pr_int_annc.HasAGMismatch()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A08");
                message.AppendLine("PR_INT_ANNC / PR_AN_GRP - AudioGroup Mismatch");
                message.AppendLine($"UID: {pr_int_annc.UID}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Database.ManualUIDs.Add(pr_int_annc.UID);
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA08Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A09 : This audit triggers when a PR_INT_ANNC is missing it's PR_AN_GRP
        internal static bool AuditA09(PR_INT_ANNC pr_int_annc) {
            if (!pr_int_annc.HasANGRP()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A09");
                message.AppendLine("Missin PR_AN_GRP");
                message.AppendLine($"UID: {pr_int_annc.UID}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.PR_AN_GRP(pr_int_annc.UID, pr_int_annc.AudioGroup));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                AuditA09Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A10 : This audit triggers when there it a duplicate PR_IANC_BD
        internal static bool AuditA10(PR_GM_IANC_BD pr_gm_ianc_bd) {
            if (pr_gm_ianc_bd.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A10");
                message.AppendLine("Duplicate PR_GM_IANC_BD");
                message.AppendLine($"UID: {pr_gm_ianc_bd.UID}");
                message.AppendLine($"Board: {pr_gm_ianc_bd.Board}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_GM_IANC_BD(pr_gm_ianc_bd));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                AuditA10Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-A11 : This audit triggers when there is a missing PR_AG_MBR
        internal static bool AuditA11(PR_GM_IANC_BD pr_gm_ianc_bd, string audiogroup) {
            if (!pr_gm_ianc_bd.HasAGMBR(audiogroup)) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-A11");
                message.AppendLine("Missing PR_AG_MBR");
                message.AppendLine($"UID: {pr_gm_ianc_bd.UID}");
                message.AppendLine($"Board: {pr_gm_ianc_bd.Board}");
                message.AppendLine($"AudioGroup: {audiogroup}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedAnnouncements++;
                ManualFixes++;
                AuditA11Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T01 : This audit triggers when a PR_TR_MBR is missing a PR_TRUNK
        internal static bool AuditT01(PR_TR_MBR pr_tr_mbr) {
            if (!pr_tr_mbr.HasTRUNK()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T01");
                message.AppendLine("PR_TR_MBR is missing PR_TRUNK");
                message.AppendLine($"UID: {pr_tr_mbr.UID}");
                message.AppendLine($"TrunkGroup: {pr_tr_mbr.TrunkGroup}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                ManualFixes++;
                AuditT01Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T02 : This audit triggers when a PR_TRUNK is missing a PR_TR_MBR
        internal static bool AuditT02(PR_TRUNK pr_trunk) {
            if (!pr_trunk.HasTRMBR()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T02");
                message.AppendLine("PR_TRUNK is missing PR_TR_MBR");
                message.AppendLine($"UID: {pr_trunk.UID}");
                message.AppendLine($"TrunkGroup: {pr_trunk.TrunkGroup}");
                message.AppendLine();
                message.AppendLine("** MANUAL FIX REQUIRED **");
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                ManualFixes++;
                AuditT02Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T03 : This audit triggers when a PR_TRUNK is missing either
        // PR_PORT_UID or PR_MOPORT
        internal static bool AuditT03(PR_TRUNK pr_trunk) {
            if (!pr_trunk.HasMOPORT() || !pr_trunk.HasPORTUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T03");
                message.AppendLine("PR_TRUNK is missing port PRECs");
                message.AppendLine($"UID: {pr_trunk.UID}");
                message.AppendLine($"TrunkGroup: {pr_trunk.TrunkGroup}");
                message.AppendLine();
                message.AppendLine(Fixer.Add.TrunkPort(pr_trunk));
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT03Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T04 : This audit triggers when there are duplicate PR_ACD_TRUNK records
        internal static bool AuditT04(PR_ACD_TRUNK pr_acd_trunk) {
            if (pr_acd_trunk.HasDuplicates()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T04");
                message.AppendLine("Duplicate PR_ACD_TRUNK record");
                message.AppendLine($"TG UID: {pr_acd_trunk.TrunkGroupUID}");
                message.AppendLine($"MEMBER UID: {pr_acd_trunk.TrunkMemberUID}");

                var pr_acd_trunks =
                    Database.PR_ACD_TRUNKs.FindAll(a => a.TrunkGroupUID == pr_acd_trunk.TrunkGroupUID &&
                        a.TrunkMemberUID == pr_acd_trunk.TrunkMemberUID);
                message.AppendLine($"COUNT: {pr_acd_trunks.Count}");

                foreach (var trunk in pr_acd_trunks) { trunk.Flagged = true; }

                message.AppendLine();

                var count = pr_acd_trunks.Count;
                while (count > 1) {
                    message.AppendLine(Fixer.Remove.PR_ACD_TRUNK(pr_acd_trunk.TrunkGroupUID, pr_acd_trunk.TrunkMemberUID));
                    count--;
                }
                
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT04Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T05 : This audit checks if there are PR_ACD_TRUNKs on trunk groups that are not measured
        internal static bool AuditT05(PR_TR_GRP pr_tr_grp) {
            if (!pr_tr_grp.Measured && pr_tr_grp.HasACDTRUNK()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T05");
                message.AppendLine("PR_ACD_TRUNK exists on unmeasured trunk group");
                message.AppendLine($"UID: {pr_tr_grp.UID}");

                var pr_acd_trunks = Database.PR_ACD_TRUNKs.FindAll(a => a.TrunkGroupUID == pr_tr_grp.UID);
                message.AppendLine($"COUNT: {pr_acd_trunks.Count}");
                message.AppendLine();

                foreach (var pr_acd_trunk in pr_acd_trunks) {
                    message.AppendLine(Fixer.Remove.PR_ACD_TRUNK(pr_acd_trunk.TrunkGroupUID, pr_acd_trunk.TrunkMemberUID));
                }

                message.AppendLine();
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT05Hits++;
                return false;
            }

            return true;
        }

        // AUDIT-T06 : This audit checks to ensure that measured trunk members have a PR_ACD_TRUNK
        internal static bool AuditT06(bool measured, PR_TR_MBR pr_tr_mbr) {
            if (measured && !pr_tr_mbr.Flagged && !pr_tr_mbr.HasACDTRUNK()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T06");
                message.AppendLine("PR_ACD_TRUNK is missing on measured trunk member");
                message.AppendLine($"UID: {pr_tr_mbr.UID}");
                message.AppendLine($"TG: {pr_tr_mbr.TrunkGroup}");
                message.AppendLine();
                var fix = Fixer.Add.PR_ACD_TRUNK(pr_tr_mbr);
                if (fix == null) {
                    var pr_tr_grp = Database.PR_TR_GRPs.Find(a => a.UID == pr_tr_mbr.TrunkGroup);
                    message.AppendLine("** NO PR_ACD_TRUNKs FOR TRUNK GROUP **");
                    message.AppendLine("** NEED TO CHANGE meas_by FIELD ON PR_TR_GRP TO 'none' (x00) USING TCM BUFFER **");
                    message.AppendLine($"** THEN USING SAT RESET MEASURED BY FIELD TO : {pr_tr_grp.MeasuredBy()} **");
                    var pr_tr_mbrs = Database.PR_TR_MBRs.FindAll(a => a.TrunkGroup == pr_tr_mbr.TrunkGroup);
                    foreach (var item in pr_tr_mbrs) item.Flagged = true;
                    ManualFixes++;
                } else { message.AppendLine(fix); }
                
                message.AppendLine();
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT06Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T07 : Check PR_MOPORT for missing PR_PORT_UID
        internal static bool AuditT07(PR_MOPORT pr_moport) {
            if (!pr_moport.HasPORTUID()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T07");
                message.AppendLine("PR_MOPORT is missing PR_PORT_UID");
                message.AppendLine($"Port: {pr_moport.Port}");
                message.AppendLine();
                if (pr_moport.HasTRUNK()) {
                    var pr_trunk = Database.PR_TRUNKs.Find(a => a.Port == pr_moport.Port);
                    message.AppendLine(Fixer.Add.TrunkPort(pr_trunk));
                } else {
                    message.AppendLine(Fixer.Remove.PR_MOPORT(pr_moport.Port));
                }
                message.AppendLine();
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT07Hits++;
                return false;
            }
            return true;
        }

        // AUDIT-T08 : Check PR_PORT_UID for missing PR_MOPORTs
        internal static bool AuditT08(PR_PORT_UID pr_port_uid) {
            if (!pr_port_uid.HasMOPORT()) {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T08");
                message.AppendLine("PR_PORT_UID is missing PR_MOPORT");
                message.AppendLine($"Port: {pr_port_uid.Port}");
                message.AppendLine($"UID: {pr_port_uid.UID}");
                message.AppendLine();
                if (pr_port_uid.HasTRUNK()) {
                    var pr_trunk = Database.PR_TRUNKs.Find(a => a.Port == pr_port_uid.Port);
                    message.AppendLine(Fixer.Add.TrunkPort(pr_trunk));
                } else {
                    message.AppendLine(Fixer.Remove.PR_PORT_UID(pr_port_uid.Port));
                }
                message.AppendLine();
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT08Hits++;
                return false;
            }
            return true;
        }

        internal static bool AuditT09(PR_MOPORT pr_moport) {
            if (!pr_moport.HasTRUNK() && pr_moport.TGUID().Substring(0, 4) == "0005") {
                var message = new StringBuilder();
                message.AppendLine("AUDIT-T09");
                message.AppendLine("PR_MOPORT is missing PR_TRUNK");
                message.AppendLine($"Port: {pr_moport.Port}");
                message.AppendLine($"UID: {pr_moport.TGUID()}");
                message.AppendLine();
                message.AppendLine(Fixer.Remove.PR_MOPORT(pr_moport.Port));
                if (pr_moport.HasPORTUID()) {
                    message.AppendLine(Fixer.Remove.PR_PORT_UID(pr_moport.Port));
                }
                message.AppendLine();
                Globals.GUI.AddOutput(message.ToString());
                Corrupted++;
                CorruptedTrunks++;
                AuditT09Hits++;
                return false;
            }

            return true;
        }
    }
}
