/*
 * Auditor3 :: Database
 * 
 * This class stores and manages the objects that are created from the PRECParser.
 * 
 * Auditor3 is developed and maintained by David McNutt -
 * 
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditor3 {
    internal static class Database {

        internal static List<PR_AMW> PR_AMWs;               // The list of PR_AMW records
        internal static List<PR_BRIDGE> PR_BRIDGEs;         // The list of PR_BRIDGE records
        internal static List<PR_BUTTON> PR_BUTTONs;         // The list of PR_BUTTON records
        internal static List<PR_EXT> PR_EXTs;               // The list of PR_EXT records
        internal static List<PR_FEXT> PR_FEXTs;             // The list of PR_FEXT records
        internal static List<PR_MOBD> PR_MOBDs;             // The list of PR_MOBD records
        internal static List<PR_MOPORT> PR_MOPORTs;         // The list of PR_MOPORT records
        internal static List<PR_OPT_STN> PR_OPT_STNs;       // The list of PR_OPT_STN records
        internal static List<PR_PORT_UID> PR_PORT_UIDs;     // The list of PR_PORT_UID records
        internal static List<PR_ST_CPS> PR_ST_CPSs;         // The list of PR_ST_CPS records
        internal static List<PR_STN> PR_STNs;               // The list of PR_STN records
        internal static List<PR_TTISET> PR_TTISETs;         // The list of PR_TTISET records
        internal static List<PR_UDATA> PR_UDATAs;           // The list of PR_UDATA records
        internal static List<PR_XMAP> PR_XMAPs;             // The list of PR_XMAP records
        //internal static List<PR_BARRY> PR_BARRYs;

        internal static List<PR_AN_GRP> PR_AN_GRPs;         // The list of PR_AN_GRP records
        internal static List<PR_AG_MBR> PR_AG_MBRs;         // The list of PR_AG_MBR records
        internal static List<PR_AUDIO_GRP> PR_AUDIO_GRPs;   // The list of PR_AUDIO_GRP records
        internal static List<PR_GM_IANC_BD> PR_GM_IANC_BDs; // The list of PR_GM_IANC_BD records
        internal static List<PR_IANC_BD> PR_IANC_BDs;       // The list of PR_IANC_BD records
        internal static List<PR_INT_ANNC> PR_INT_ANNCs;     // The list of PR_INT_ANNC records

        internal static List<PR_ACD_TRUNK> PR_ACD_TRUNKs;   // The list of PR_ACD_TRUNK records
        internal static List<PR_TR_GRP> PR_TR_GRPs;         // The list of PR_TR_GRP records
        internal static List<PR_TR_MBR> PR_TR_MBRs;         // The list of PR_TR_MBR records
        internal static List<PR_TRUNK> PR_TRUNKs;           // The list of PR_TRUNK records

        internal static List<MissingStationType> Missing;   // The list of missing station types

        internal static List<string> ManualPorts;           // List of ports that need manual fixing
        internal static List<string> ManualUIDs;            // List of UIDs that need manual fixing

        internal static int FreePortIndex;                  // Index used for checking for free ports

        // This method initializes the database during application startup
        internal static void Initialize() {
            PR_AMWs = new List<PR_AMW>();
            PR_BRIDGEs = new List<PR_BRIDGE>();
            PR_BUTTONs = new List<PR_BUTTON>();
            PR_EXTs = new List<PR_EXT>();
            PR_FEXTs = new List<PR_FEXT>();
            PR_MOBDs = new List<PR_MOBD>();
            PR_MOPORTs = new List<PR_MOPORT>();
            PR_OPT_STNs = new List<PR_OPT_STN>();
            PR_PORT_UIDs = new List<PR_PORT_UID>();
            PR_ST_CPSs = new List<PR_ST_CPS>();
            PR_STNs = new List<PR_STN>();
            PR_TTISETs = new List<PR_TTISET>();
            PR_UDATAs = new List<PR_UDATA>();
            PR_XMAPs = new List<PR_XMAP>();

            PR_AN_GRPs = new List<PR_AN_GRP>();
            PR_AG_MBRs = new List<PR_AG_MBR>();
            PR_GM_IANC_BDs = new List<PR_GM_IANC_BD>();
            PR_IANC_BDs = new List<PR_IANC_BD>();
            PR_INT_ANNCs = new List<PR_INT_ANNC>();
            PR_AUDIO_GRPs = new List<PR_AUDIO_GRP>();

            PR_ACD_TRUNKs = new List<PR_ACD_TRUNK>();
            PR_TR_GRPs = new List<PR_TR_GRP>();
            PR_TR_MBRs = new List<PR_TR_MBR>();
            PR_TRUNKs = new List<PR_TRUNK>();

            Missing = new List<MissingStationType>();
            FreePortIndex = 0;

            ManualPorts = new List<string>();
            ManualUIDs = new List<string>();
        }

        // This method is used to show the database totals
        internal static void ShowTotals() {
            // Create a stringbuilder for the counts
            var counts = new StringBuilder();

            // Display the counts
            if (Globals.STATION_AUDITS) {
                counts.AppendLine($"PR_AMW        : {PR_AMWs.Count}");
                counts.AppendLine($"PR_BRIDGE     : {PR_BRIDGEs.Count}");
                counts.AppendLine($"PR_BUTTON     : {PR_BUTTONs.Count}");
                counts.AppendLine($"PR_EXT        : {PR_EXTs.Count}");
                counts.AppendLine($"PR_FEXT       : {PR_FEXTs.Count}");
                counts.AppendLine($"PR_MOBD       : {PR_MOBDs.Count}");
                counts.AppendLine($"PR_MOPORT     : {PR_MOPORTs.Count}");
                counts.AppendLine($"PR_OPT_STN    : {PR_OPT_STNs.Count}");
                counts.AppendLine($"PR_PORT_UID   : {PR_PORT_UIDs.Count}");
                counts.AppendLine($"PR_ST_CPS     : {PR_ST_CPSs.Count}");
                counts.AppendLine($"PR_STN        : {PR_STNs.Count}");
                counts.AppendLine($"PR_TTISET     : {PR_TTISETs.Count}");
                counts.AppendLine($"PR_UDATA      : {PR_UDATAs.Count}");
                counts.AppendLine($"PR_XMAP       : {PR_XMAPs.Count}");
                counts.AppendLine();
            }
            if (Globals.ANNOUNCEMENT_AUDITS) {
                counts.AppendLine($"PR_AG_MBR     : {PR_AG_MBRs.Count}");
                counts.AppendLine($"PR_AN_GRP     : {PR_AN_GRPs.Count}");
                counts.AppendLine($"PR_AUDIO_GRP  : {PR_AUDIO_GRPs.Count}");
                counts.AppendLine($"PR_EXT        : {PR_EXTs.Count}");
                counts.AppendLine($"PR_GM_IANC_BD : {PR_GM_IANC_BDs.Count}");
                counts.AppendLine($"PR_IANC_BD    : {PR_IANC_BDs.Count}");
                counts.AppendLine($"PR_INT_ANNC   : {PR_INT_ANNCs.Count}");
                counts.AppendLine($"PR_UDATA      : {PR_UDATAs.Count}");
                counts.AppendLine();
            }
            if (Globals.TRUNK_AUDITS) {
                counts.AppendLine($"PR_ACD_TRUNK  : {PR_ACD_TRUNKs.Count}");
                counts.AppendLine($"PR_MOPORT     : {PR_MOPORTs.Count}");
                counts.AppendLine($"PR_PORT_UID   : {PR_PORT_UIDs.Count}");
                counts.AppendLine($"PR_TR_GRP     : {PR_TR_GRPs.Count}");
                counts.AppendLine($"PR_TR_MBR     : {PR_TR_MBRs.Count}");
                counts.AppendLine($"PR_TRUNK      : {PR_TRUNKs.Count}");
                counts.AppendLine();
            }

            // Display the counts
            Globals.GUI.AddOutput(counts.ToString());
        }

        // This method is used to validate all the required PRECs have been provided
        internal static bool ValidatePRECs() {
            var success = true;

            if (Globals.STATION_AUDITS && (PR_BUTTONs.Count == 0 ||
                PR_EXTs.Count == 0 || PR_FEXTs.Count == 0 || PR_MOPORTs.Count == 0 ||
                PR_PORT_UIDs.Count == 0 || PR_ST_CPSs.Count == 0 || PR_STNs.Count == 0 ||
                PR_UDATAs.Count == 0)) {
                success = false;
            }
            
            if (Globals.STATION_AUDITS && (PR_BRIDGEs.Count == 0 && 
                PR_BUTTONs.Where(a => a.Bridged).Count() > 0)) {
                success = false;
            }

            if (Globals.ANNOUNCEMENT_AUDITS && (PR_GM_IANC_BDs.Count == 0 || PR_IANC_BDs.Count == 0 ||
                PR_INT_ANNCs.Count == 0 || PR_UDATAs.Count == 0 || PR_EXTs.Count == 0)) {
                success = false;
            }

            if (Globals.ANNOUNCEMENT_AUDITS && 
                (PR_INT_ANNCs.FindAll(a => a.AudioGroup != "00").Count != 0) && PR_AG_MBRs.Count == 0) {
                success = false;
            }

            if (Globals.TRUNK_AUDITS && (PR_MOPORTs.Count == 0 || PR_PORT_UIDs.Count == 0 ||
                PR_TR_MBRs.Count == 0 || PR_TRUNKs.Count == 0 || PR_TR_GRPs.Count == 0)) {
                success = false;
            }

            if (!success) {
                Globals.GUI.AddOutput("** YOU HAVE NOT PROVIDED ALL THE REQUIRED PRECS **");
                Globals.GUI.AddOutput("** PLEASE REFER TO THE PREC LIST UNDER THE HELP MENU **");
                Globals.GUI.AddOutput("");
            }
            return success;
        }

        // This method is for adding a missing station type record
        internal static void AddMissingStationType(string type, string uid) {
            var missing = new MissingStationType { Type = type, UID = uid };
            Missing.Add(missing);
        }

        // This method is used to process missing station types
        internal static void ProcessMissingStationTypes() {
            // Check if we have any
            if (Missing.Count == 0) return;

            // Loop through them
            foreach (var missing in Missing) {
                var pr_st_cps = PR_ST_CPSs.Find(a => a.UID == missing.UID);
                if (pr_st_cps != null) { missing.Port = pr_st_cps.Port; }
                if (missing.Port == null || missing.Port == Globals.NULL_PORT) {
                    var pr_port_uid = PR_PORT_UIDs.Find(a => a.UID == missing.UID);
                    if (pr_port_uid != null) missing.Port = pr_port_uid.Port;
                }
                if (missing.Port != null && missing.Port != Globals.NULL_PORT) {
                    var pr_moport = PR_MOPORTs.Find(a => a.Port == missing.Port);
                    if (pr_moport != null) missing.MO = pr_moport.MO;
                }
                Globals.GUI.AddOutput($"MISSING STATION TYPE - TYPE: {missing.Type} PORT: {missing.Port} MO: {missing.MO}");
            }
            Globals.GUI.AddOutput("");
        }

        // This method is for finding an unused IP port
        internal static string FindUnusedIPPort() {
            // Declare variables we need
            var found = false;
            string port = null;

            // Loop while we havent found a port
            while (!found) {
                // Create the port we want to check for using the index
                var check = $"7f{FreePortIndex.ToString("X").PadLeft(6, '0').ToLower()}";

                // Increment the index
                FreePortIndex++;

                // Check if we have any precs using this port
                var pr_st_cps = PR_ST_CPSs.Find(a => a.Port == check);
                var pr_port_uid = PR_PORT_UIDs.Find(a => a.Port == check);
                var pr_moport = PR_MOPORTs.Find(a => a.Port == check);
                if (pr_st_cps != null || pr_port_uid != null || pr_moport != null) continue;

                // We have a winner
                found = true;
                port = check;
            }

            // Return the found port
            return port;
        }
    }
}
