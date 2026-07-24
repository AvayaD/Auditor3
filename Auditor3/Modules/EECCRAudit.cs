/*
 * Auditor3 :: EECCRAudit
 * 
 * This class defines the process that runs various OSSI commands and checks for EECCRs.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.IO;
using Renci.SshNet;

namespace Auditor3 {
    internal static class EECCRAudit {
        private static string _results;         // Variable for storing the OSSI output as we go
        private static int _eeccrs;             // The total EECCRs we found
        private static ShellStream _shell;

        // String definition of EECCR to hunt for
        private const string _eeccr = "Error encountered, can't complete request";

        // List of items to run list commands against during EECCR audit
        private static readonly string[] EECCRList = new string[] { "station", "agent-loginID", "announcement", "extension-type", "hunt-group",
            "off-pbx-telephone station-mapping", "trunk-group", "signaling-group", "survivable-processor", "vector", "route-pattern",
            "vrt", "partition-route-table", "data-module", "audio-group", "ip-interface all", "ip-route", "ethernet-options",
            "media-gateway", "node-routing", "node-names", "extended-pickup-group", "cor", "abbreviated-dialing personal",
            "abbreviated-dialing group", "aca-parameters", "aar analysis", "ars analysis", "configuration all", "configuration ds1",
            "coverage answer-group", "coverage path", "intercom-group", "personal-CO-line", "pickup-group", "term-ext-group",
            "toll all", "toll restricted-call", "toll toll-list", "toll unrestricted-call", "uniform-dialplan", "vdn", "multimedia ip-stations",
            "multimedia ip-unregistered" };

        // List of items to run display commands against during EECCR audit
        private static readonly string[] EECCRDisplay = new string[] { "system-parameters customer-options", "system-parameters features",
            "system-parameters hospitality", "system-parameters ip-options", "system-parameters maintenance",
            "system-parameters mlpp","system-parameters security", "system-parameters cdr",
            "ip-services", "music-sources" };

        // This method is used to start the EECCR auditor
        internal static void Start() {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.EECCR_AUDIT;
            Globals.START_TIME = DateTime.Now;
            Globals.GUI.AddStatus("EECCR audit is starting");

            try {
                Run();
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while running EECCR audit", error);
            }

            Globals.CM.CloseTCM(_shell);
            Globals.CM.CloseOSSI(_shell);
            _shell.Dispose();
            _shell = null;

            Globals.GUI.Idle();
        }

        // This method runs the actual audit
        private static void Run() {
            // Make sure the site connection is up
            if (!Globals.CM.Connected()) {
                var connect = Globals.CM.Connect();
                if (!connect) return;
            }

            // Initialize the results and report fields
            _results = "";
            _eeccrs = 0;            

            // Get a shell from the connection
            _shell = Globals.CM.Shell();
            if (_shell == null) { return; }

            // Add the header to the report
            Globals.GUI.AddOutput($"EECCR Audit v{Globals.VERSION()}");
            Globals.GUI.AddOutput("");
            Globals.GUI.AddOutput($"CM_RELEASE    : {Globals.CM_RELEASE}");
            Globals.GUI.AddOutput($"STATIONS      : {Globals.STATION_AUDITS}");
            Globals.GUI.AddOutput($"TRUNKS        : {Globals.TRUNK_AUDITS}");
            Globals.GUI.AddOutput($"ANNOUNCEMENTS : {Globals.ANNOUNCEMENT_AUDITS}");
            Globals.GUI.AddOutput($"START TIME    : {Globals.TIMESTAMP()}");
            Globals.GUI.AddOutput("");

            // Open OSSI
            if (!Globals.CM.OpenOSSI(_shell)) return;

            // Run all of the commands
            foreach (var item in EECCRList) if (!RunList(item)) return;
            foreach (var item in EECCRDisplay) if (!RunDisplay(item)) return;

            Globals.GUI.AddOutput("");
            Globals.GUI.AddOutput("EECCRS: " + _eeccrs);
            Globals.GUI.AddOutput("");

            var runtime = (DateTime.Now - Globals.START_TIME).TotalSeconds;
            Globals.GUI.AddOutput($"EECCR audit completed in {runtime} seconds");

            // Write the report
            var output = Globals.GUI.GetOutput();
            var report = Globals.REPORT("eeccrs");
            var writer = new StreamWriter(report);
            writer.Write(output);
            writer.Close();

            Globals.GUI.AddStatus($"EECCR audit report generated at {report}");
        }

        // Method for running a list command
        private static bool RunList(string item) {
            Globals.GUI.AddOutput("- Running list " + item);

            // Run the command and make sure we got results
            var result = Globals.CM.RunSATCommand(_shell, $"list {item}");
            if (result == null) return false;

            // Check if we have an EECCR
            if (result.Contains(_eeccr)) {
                Globals.GUI.AddOutput("EECCR: list " + item);
                Globals.GUI.AddOutput(Globals.LAST_LINES(result));
                _eeccrs++;
            }

            // Store the results
            _results += result += Environment.NewLine;
            return true;
        }

        // Method for running a display command
        private static bool RunDisplay(string item) {
            Globals.GUI.AddOutput("- Running display " + item);

            // Run the command and make sure we got results
            var result = Globals.CM.RunSATCommand(_shell, $"display {item}");
            if (result == null) return false;

            // Check if we have an EECCR
            if (result.Contains(_eeccr)) {
                Globals.GUI.AddOutput("EECCR: display " + item);
                Globals.GUI.AddOutput(Globals.LAST_LINES(result));
                _eeccrs++;
            }

            // Store the results
            _results += result += Environment.NewLine;
            return true;
        }
    }
}
