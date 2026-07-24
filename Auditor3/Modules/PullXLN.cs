/*
 * Auditor3 :: PullXLN
 * 
 * This class defines the process that pulls the XLN file from a live system.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;

namespace Auditor3 {
    internal static class PullXLN {
        // Method that starts the process
        internal static void Start() {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.PULLXLN;
            Globals.START_TIME = DateTime.Now;

            try {
                Run();
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while pulling the XLN file", error);
            }

            Globals.GUI.Idle();
        }

        // Method for running the process
        internal static void Run() {
            // Make sure the site connection is up
            if (!Globals.CM.Connected()) {
                if (!Globals.CM.Connect()) return;
            }

            // Get a shell from the connection
            var shell = Globals.CM.Shell();
            if (shell == null) { return; }

            Globals.GUI.AddStatus("Copying XLN file to init user home directory");
            Globals.CM.CopyFile("/etc/opt/defty/xln1", "/var/home/init/auditor_xln1");

            Globals.GUI.AddStatus("Compressing XLN file");
            Globals.CM.GZipFile("/var/home/init/auditor_xln1");

            Globals.GUI.AddStatus("Downloading compressed XLN file to local PC");
            Globals.CM.RecieveFile("/var/home/init/auditor_xln1.gz", Globals.XLN_FILE);

            Globals.GUI.AddStatus("XLN retrieval process complete");
        }
    }
}
