/*
 * Auditor3 :: Repairer
 * 
 * This class defines the process that runs the fix commands from the Auditor process.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.IO;
using Renci.SshNet;

namespace Auditor3 {
    internal static class Repairer {
        private static ShellStream _shell;

        // This method is used to start the repairer
        internal static void Start() {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.REPAIR;
            Globals.START_TIME = DateTime.Now;
            Globals.GUI.AddStatus("Repair machine is starting");

            try {
                Run();
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while running repair commands", error);
            }

            Globals.CM.CloseTCM(_shell);
            Globals.CM.CloseOSSI(_shell);
            _shell.Dispose();
            _shell = null;

            // Write the report
            var output = Globals.GUI.GetOutput();
            var report = Globals.REPORT("repair");
            var writer = new StreamWriter(report);
            writer.Write(output);
            writer.Close();

            Globals.GUI.AddStatus($"Repair machine report generated at {report}");

            Globals.GUI.Idle();
        }

        // This method is used to run the collector
        internal static void Run() {
            // Make sure the site connection is up
            if (!Globals.CM.Connected()) {
                if (!Globals.CM.Connect()) return;
            }

            // Get a shell from the connection
            _shell = Globals.CM.Shell();
            if (_shell == null) { return; }

            // Add the header to the report
            Globals.GUI.AddOutput($"Repair Machine v{Globals.VERSION()}");
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

            Globals.GUI.AddOutput($"Total repair commands: {Fixer.FixLines.Count}");
            Globals.GUI.AddStatus("Running repair commands");

            var output = "";

            Audits.ToCheck = Fixer.FixLines.Count;
            Audits.Checked = 0;

            foreach (var cmd in Fixer.FixLines) {
                if (Globals.CANCEL) break;
                Audits.Checked++;
                var tcm = Globals.CM.RunTCMCommand(_shell, cmd);
                if (tcm == null) return;
                else output += tcm;

                if ((Audits.Checked % 10) == 0 && !Globals.WYLD_STALLYN) {
                    Globals.GUI.AddOutput(output);
                    output = "";
                }
            }

            if (!Globals.WYLD_STALLYN) {
                Globals.GUI.AddOutput(output);
                Globals.GUI.AddOutput("");
            }            

            var runtime = (DateTime.Now - Globals.START_TIME).TotalSeconds;
            Globals.GUI.AddOutput($"Repair machine completed in {runtime} seconds");
        }
    }
}
