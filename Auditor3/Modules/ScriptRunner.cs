/*
 * Auditor3 :: ScriptRunner
 * 
 * This class defines the process that runs a custom fix script.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.IO;

namespace Auditor3 {
    internal static class ScriptRunner {
        // Method for running the custom script from a local file
        internal static void RunLocal(string filename) {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.LOADSCRIPT;
            Globals.START_TIME = DateTime.Now;

            Fixer.Initialize();

            try {
                Globals.GUI.AddStatus($"Reading custom fix script from {filename}");
                var file = new StreamReader(filename);
                var fixes = file.ReadToEnd();
                file.Close();
                Globals.GUI.AddStatus("Adding fixes to table");
                Fixer.AddFix(fixes);

                if (Fixer.FixLines.Count == 0) Globals.GUI.Error("No fixes were detected in the provided file");
                else Repairer.Start();

            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while executing custom fix script", error);
            }

            Globals.GUI.Idle();
        }

        // Method for running the custom script from a ToolsA file
        internal static void RunToolsA(string filename) {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.LOADSCRIPT;
            Globals.START_TIME = DateTime.Now;

            Fixer.Initialize();

            try {
                Globals.GUI.AddStatus($"Reading custom fix script from {filename}");

                var fixes = Globals.TOOLSA.Cat(filename);

                Globals.GUI.AddStatus("Adding fixes to table");
                Fixer.AddFix(fixes);

                if (Fixer.FixLines.Count == 0) Globals.GUI.Error("No fixes were detected in the provided file");
                else Repairer.Start();

            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while executing custom fix script", error);
            }

            Globals.GUI.Idle();
        }
    }
}
