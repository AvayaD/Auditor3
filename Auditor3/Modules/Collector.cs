/*
 * Auditor3 :: Collector
 * 
 * This class defines the process that retrieves PRECs from a CM.
 * 
 * Auditor3 is developed and maintained by David McNutt
 */

using System;
using System.IO;
using System.Text.RegularExpressions;
using Renci.SshNet;

namespace Auditor3 {
    internal static class Collector {

        private static string _precs;
        private static ShellStream _shell;

        // This method is used to start the collector
        internal static void Start() {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.COLLECT;
            Globals.START_TIME = DateTime.Now;
            Globals.GUI.AddStatus("PREC collector is starting");

            Globals.PRECS_LOADED = false;
            Globals.AUDIT_COMPLETE = false;

            _precs = "";

            try {
                Run();                
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while collecting PRECs", error);
            }

            Globals.CM.CloseTCM(_shell);
            Globals.CM.CloseOSSI(_shell);
            _shell.Dispose();
            _shell = null;

            Globals.GUI.Idle();
        }

        // This method is used to run the collector
        internal static void Run() {
            // Make sure the site connection is up
            if (!Globals.CM.Connected())
                if (!Globals.CM.Connect()) return;

            // Get a shell from the connection
            _shell = Globals.CM.Shell();
            if (_shell == null) { return; }            

            // Add the header to the report
            Globals.GUI.AddOutput($"PREC Collector v{Globals.VERSION()}");
            Globals.GUI.AddOutput("");
            Globals.GUI.AddOutput($"CM_RELEASE    : {Globals.CM_RELEASE}");
            Globals.GUI.AddOutput($"STATIONS      : {Globals.STATION_AUDITS}");
            Globals.GUI.AddOutput($"TRUNKS        : {Globals.TRUNK_AUDITS}");
            Globals.GUI.AddOutput($"ANNOUNCEMENTS : {Globals.ANNOUNCEMENT_AUDITS}");
            Globals.GUI.AddOutput($"START TIME    : {Globals.TIMESTAMP()}");
            Globals.GUI.AddOutput("");            

            // Open OSSI and TCM
            if (!Globals.CM.OpenOSSI(_shell)) return;
            if (!Globals.CM.OpenTCM(_shell)) return;

            if (Globals.STATION_AUDITS) {
                if (!PullPREC("PR_AMW")) return;
                if (!PullPREC("PR_BRIDGE")) return;
                if (!PullPREC("PR_BUTTON")) return;
                if (!PullPREC("PR_EXT")) return;
                if (!PullPREC("PR_FEXT")) return;
                if (!PullPREC("PR_MOBD")) return;
                if (!PullPREC("PR_MOPORT")) return;
                if (!PullPREC("PR_OPT_STN")) return;
                if (!PullPREC("PR_PORT_UID")) return;
                if (!PullPREC("PR_ST_CPS")) return;
                if (!PullPREC("PR_STN")) return;
                if (!PullPREC("PR_TTISET")) return;
                if (!PullPREC("PR_UDATA")) return;
                if (!PullPREC("PR_XMAP")) return;
            }

            if (Globals.ANNOUNCEMENT_AUDITS) {
                if (!Globals.STATION_AUDITS) {
                    if (!PullPREC("PR_EXT")) return;
                    if (!PullPREC("PR_UDATA")) return;
                }
                if (!PullPREC("x4c90")) return;
                if (!PullPREC("x4c91")) return;
                if (!PullPREC("PR_AN_GRP")) return;
                if (!PullPREC("PR_GM_IANC_BD")) return;
                if (!PullPREC("PR_IANC_BD")) return;
                if (!PullPREC("PR_INT_ANNC")) return;
            }

            if (Globals.TRUNK_AUDITS) {
                if (!Globals.STATION_AUDITS) {
                    if (!PullPREC("PR_MOPORT")) return;
                    if (!PullPREC("PR_PORT_UID")) return;
                }
                if (!PullPREC("PR_ACD_TRUNK")) return;
                if (!PullPREC("PR_TR_GRP")) return;
                if (!PullPREC("PR_TR_MBR")) return;
                if (!PullPREC("PR_TRUNK")) return;
            }

            Globals.GUI.AddStatus("Cleaning pulled data");
            _precs = Globals.CLEAN(_precs);

            Globals.GUI.SetPRECs(_precs);

            var precs = new StreamWriter(Globals.PRECS_FILE);
            precs.Write(_precs);
            precs.Close();

            var runtime = (DateTime.Now - Globals.START_TIME).TotalSeconds;
            Globals.GUI.AddOutput($"Collector completed in {runtime} seconds");

            _precs = null;
            Globals.PRECS_LOADED = true;
        }

        // This method is for running a PREC command
        private static bool PullPREC(string prec) {
            Globals.GUI.AddOutput($"Pulling {prec}");

            var cmd = $"prec {prec.ToLower()} nr";

            string result = null;

            // Loop for 3 tries pulling each PREC
            for (var i = 0; i <= 3; i++) {
                result = Globals.CM.RunTCMCommand(_shell, cmd);
                if (result == null) return false;
                if (ValidatePRECData(prec, cmd, result)) break;
                Globals.GUI.Error($"TCM prec nread command failed. Attempt: {i}");
                if (i == 3) {
                    Globals.GUI.Error("TCM prec nread command failed 3 times, aborting");
                    return false;
                }
            }

            _precs += result + Environment.NewLine;
            return true;
        }

        // Method for checking if we have valid PREC data response
        private static bool ValidatePRECData(string prec, string command, string output) {
            // Does the output have the PREC name, if it does, its likely valid
            if (output.Contains(prec)) return true;

            // Split the lines and make sure they are all either prompt lines or null
            var prompt = new Regex("tcm[0-9]+>");
            var lines = output.Split('\n');
            foreach (var line in lines) {
                if (prompt.IsMatch(line) || string.IsNullOrEmpty(line) || line.Contains(command)) continue;
                Globals.GUI.AddStatus("PREC Output Validation Failure::");
                Globals.GUI.AddStatus(line);
                return false;
            }

            return true;
        }
    }
}
