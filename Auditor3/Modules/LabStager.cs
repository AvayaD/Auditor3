/*
 * Auditor3 :: LabStager
 * 
 * This class defines the process that stages a lab.
 * 
 * Auditor3 is developed and maintained by David McNutt
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Renci.SshNet;

namespace Auditor3 {
    internal static class LabStager {
        internal static string CMRelease;
        internal static string IP;
        internal static bool Patching;
        internal static string Patch;
        internal static bool LoadingXLN;
        internal static string XLNFile;
        internal static bool LocalXLNFile;
        internal static bool XLNBackup;

        private static LabStagerConnection _lab;
        private static ShellStream _shell;

        internal static string ReleaseString;
        internal static string DetectedRelease;
        internal static string PatchString;
        internal static string PatchNumber;
        internal static List<string> DeactivatedPatches;

        internal static string ToolsAFilename;

        // Method for initializing the process
        internal static void Initialize() {
            CMRelease = "";
            IP = "";
            Patching = false;
            Patch = "";
            LoadingXLN = false;
            XLNFile = "";
            LocalXLNFile = false;
            XLNBackup = false;
            _lab = null;
            ReleaseString = "";
            DetectedRelease = "";
            PatchString = "";
            PatchNumber = "";
            DeactivatedPatches = new List<string>();
        }

        // Method for starting the process
        internal static void Start() {
            // Set the state/process, set the start time, and add a status message
            Globals.STATE = State.RUNNING;
            Globals.PROCESS = Process.STAGELAB;
            Globals.START_TIME = DateTime.Now;
            Globals.GUI.AddStatus("Lab Stager is starting");

            try {
                Run();
            } catch (Exception error) {
                Globals.GUI.Error("An exception occured while staging the lab", error);
            }

            Globals.GUI.AddOutput("");

            // Get the runtime and add an output message
            var runtime = (DateTime.Now - Globals.START_TIME).TotalSeconds;
            Globals.GUI.AddOutput($"Lab staged in {runtime} seconds");

            // Write the report
            var output = Globals.GUI.GetOutput();
            var report = Globals.REPORT("labstager");
            var writer = new StreamWriter(report);
            writer.Write(output);
            writer.Close();

            Globals.GUI.AddStatus($"Lab Stager report generated at {report}");

            _lab.Disconnect();
            _lab = null;
            Globals.GUI.Idle();
        }

        // Method for running the process
        private static void Run() {
            // Add the header to the report
            Globals.GUI.AddOutput($"Lab Stager v{Globals.VERSION()}");
            Globals.GUI.AddOutput("");
            Globals.GUI.AddOutput($"CM     : {CMRelease}");
            Globals.GUI.AddOutput($"IP     : {IP}");
            Globals.GUI.AddOutput($"XLN    : {LoadingXLN}");
            Globals.GUI.AddOutput($"BACKUP : {XLNBackup}");
            Globals.GUI.AddOutput($"FILE   : {XLNFile}");
            Globals.GUI.AddOutput($"PATCH  : {Patching}");
            Globals.GUI.AddOutput($"PATCH# : {Patch}");
            Globals.GUI.AddOutput("");

            // Drop the CM connection, just in case someone is connected to a live
            // CM and thinks this will move the Auditor's main connection, which it
            // won't, as we use a specific connection to avoid any accidental
            // customer system impacts
            if (Globals.CM.Connected()) Globals.CM.Disconnect();

            // Create a connection to the lab
            _lab = new LabStagerConnection();
            if (!_lab.Connect(IP)) return;

            _shell = _lab.Shell();
            if (_shell == null) {
                Globals.GUI.Error("Failed to obtain shell");
            }

            // Handle the swversion
            HandleSwversion();

            var otherpatches = "";
            foreach (var patch in DeactivatedPatches) otherpatches += $"{patch}  ";

            Globals.GUI.AddOutput($"RELEASE STRING      : {ReleaseString}");
            Globals.GUI.AddOutput($"DETECTED RELEASE    : {DetectedRelease}");
            Globals.GUI.AddOutput($"PATCH STRING        : {PatchString}");
            Globals.GUI.AddOutput($"PATCH NUMBER        : {PatchNumber}");
            Globals.GUI.AddOutput($"DEACTIVATED PATCHES : {otherpatches}");
            Globals.GUI.AddOutput("");

            // Make sure the CM version is correct
            if (DetectedRelease != CMRelease) {
                Globals.GUI.Error("Detected CM release on lab does not match expected release");
                return;
            }

            SetMOTD();
            if (Patching) HandlePatching();
            if (LoadingXLN) HandleLoadingXLN();
        }

        // Method for handling the swversion
        private static void HandleSwversion() {
            Globals.GUI.AddStatus("Processing swversion");
            var swversion = _lab.Swversion();
            var split = swversion.Split('\n');
            foreach (var line in split) {
                if (line.Contains("CM Reports as")) {
                    ReleaseString = line.Split(' ').Where(a => !string.IsNullOrEmpty(a)).ToArray()[3];
                    ProcessReleaseString();
                } else if (line.Contains("activated") && !line.Contains("hot") && !line.Contains("VMWT") &&
                    !line.Contains("KERNEL") && !line.Contains("PLAT")) {
                    PatchString = line.Split(' ')[0];
                    PatchNumber = PatchString.Split('-')[1];
                } else if (line.Contains("unpacked") && !line.Contains("hot") && !line.Contains("VMWT") &&
                    !line.Contains("KERNEL") && !line.Contains("PLAT")) {
                    var patch = line.Split(' ')[0];
                    DeactivatedPatches.Add(patch);
                }
            }
        }

        // Method for converting the release string
        private static void ProcessReleaseString() {
            switch (ReleaseString) {
                case "R016x.03.0.141.0":
                    DetectedRelease = "6.3";
                    break;

                case "R017x.00.0.441.0":
                    DetectedRelease = "7.0";
                    break;

                case "R017x.01.0.532.0":
                    DetectedRelease = "7.1";
                    break;

                case "R018x.00.0.822.0":
                    DetectedRelease = "8.0";
                    break;

                case "R018x.01.0.890.0":
                    DetectedRelease = "8.1";
                    break;

                default:
                    DetectedRelease = "";
                    break;
            }
        }

        // Method for handling loading the XLN file
        private static void HandleLoadingXLN() {
            Globals.GUI.AddStatus("Processing loading XLN");

            Globals.GUI.AddOutput("LAB XLN STAGING");
            Globals.GUI.AddOutput("===============");
            Globals.GUI.AddOutput("");

            bool success;

            if (LocalXLNFile) success = CopyLocalXLN();
            else success = CopyToolsAXLN();

            if (!success) return;

            if (XLNBackup) success = ProcessXLNBackup();
            else success = ProcessXLN();

            if (!success) return;

            Globals.GUI.AddOutput("-Performing reset system 4 to load translations");
            _shell.WriteLine("/opt/ecs/sbin/drestart 1 4");
            WaitForCM();
        }

        // Method for copying the XLN from the local PC
        private static bool CopyLocalXLN() {
            if (!File.Exists(XLNFile)) {
                Globals.GUI.Error($"File {XLNFile} does not exist");
                return false;
            }

            Globals.GUI.AddOutput($"-Transferring XLN file {XLNFile} from local PC");

            var filename = XLNFile.Split('\\').Last();
            ToolsAFilename = $"/var/home/init/{filename}";
            return _lab.SendFile(XLNFile, ToolsAFilename);
        }

        // Method for copying the XLN from ToolsA
        private static bool CopyToolsAXLN() {
            Globals.GUI.AddOutput($"-Transferring XLN file {XLNFile} from ToolsA");

            var filename = XLNFile.Split('/').Last();
            ToolsAFilename = $"/var/home/init/{filename}";

            var shellReader = new StreamReader(_shell);
            var shellWriter = new StreamWriter(_shell);

            shellWriter.AutoFlush = true;

            // Flush two lines
            shellWriter.WriteLine();
            shellWriter.WriteLine();

            var cmd = $"/usr/bin/scp {Globals.USER_DATA.ToolsAUsername}@135.11.60.87:{XLNFile} {ToolsAFilename}";
            Globals.GUI.AddOutput(cmd);

            shellWriter.WriteLine(cmd);
            var first = _shell.Expect(new Regex("Password:|Are you sure you want to continue connecting"), new TimeSpan(0, 0, 10));
            if (string.IsNullOrEmpty(first)) {
                Globals.GUI.Error("Failed to authenticate scp to ToolsA");
                return false;
            }

            if (first.Contains("Are you sure you want to continue connecting")) {
                shellWriter.WriteLine("yes");
                var recheck = _shell.Expect("Password:");
                if (string.IsNullOrEmpty(recheck)) {
                    Globals.GUI.Error("Failed to authenticate scp to ToolsA");
                    return false;
                }
            }

            shellWriter.WriteLine(Globals.USER_DATA.ToolsAPassword);
            var second = _shell.Expect("100%", new TimeSpan(0, 2, 0));
            if (string.IsNullOrEmpty(second)) {
                Globals.GUI.Error("Failed to transfer file from ToolsA");
                return false;
            }

            return true;
        }

        // Method for processing an uploaded XLN
        private static bool ProcessXLN() {
            Globals.GUI.AddOutput($"-Processing uploaded XLN {ToolsAFilename}");

            if (ToolsAFilename.EndsWith(".gz")) {
                Globals.GUI.AddOutput($"-Running 'gunzip' on uploaded file");
                _lab.GUnZipFile(ToolsAFilename);
                ToolsAFilename = ToolsAFilename.Substring(0, ToolsAFilename.Length - 3);
            }

            CopyXLNToDefty();

            return true;
        }

        // Method for processing an uploaded backup
        private static bool ProcessXLNBackup() {
            Globals.GUI.AddOutput($"-Processing uploaded backup {ToolsAFilename}");

            var filename = ToolsAFilename.Split('/').Last();
            var directory = $"/var/home/init/{Globals.USER_DATA.ToolsAUsername.ToLower()}_{DateTime.Now.ToString("MMdd_HHmm")}/";

            Globals.GUI.AddOutput($"-Creating directory {directory}");
            _lab.CreateDirectory(directory);

            Globals.GUI.AddOutput("-Copying backup to new directory");
            _lab.CopyFile(ToolsAFilename, $"{directory}{filename}");
            ToolsAFilename = directory + filename;

            Globals.GUI.AddOutput("-Extracting backup");
            _lab.ExtractCompressedTAR(ToolsAFilename, directory);

            ToolsAFilename = $"{directory}etc/opt/defty/xln1";

            CopyXLNToDefty();

            return true;
        }

        // Method for switching to sroot to copy XLN file in place
        private static bool CopyXLNToDefty() {
            Globals.GUI.AddOutput("-Switching to sroot account");

            var shellReader = new StreamReader(_shell);
            var shellWriter = new StreamWriter(_shell);

            shellWriter.AutoFlush = true;

            // Flush two lines
            shellWriter.WriteLine();
            shellWriter.WriteLine();

            if (!GoSroot(shellWriter)) return false;

            Globals.GUI.AddOutput("-Copying XLN file to /etc/opt/defty/xln1");
            shellWriter.WriteLine($"/bin/cp {ToolsAFilename} /etc/opt/defty/xln1");
            shellWriter.WriteLine("y");
            Thread.Sleep(2000);
            Globals.GUI.AddOutput("-Leaving sroot account");
            shellWriter.WriteLine("exit");

            return true;
        }

        // Method for handling patching
        private static void HandlePatching() {
            Globals.GUI.AddStatus("Processing patching");

            Globals.GUI.AddOutput("LAB PATCH STAGING");
            Globals.GUI.AddOutput("=================");
            Globals.GUI.AddOutput("");

            if (PatchNumber == Patch) {
                Globals.GUI.AddOutput("-Current patch already matches requested patch");
            } else {
                if (!string.IsNullOrEmpty(PatchNumber)) {
                    Globals.GUI.AddOutput("-Lab has patch that needs to be deactivated");
                    if (!DeactivatePatch()) return;
                    if (!WaitForCM()) return;
                }

                var check = "";
                foreach (var patch in DeactivatedPatches) {
                    var number = patch.Split('-')[1];
                    if (number == Patch) check = patch;
                }
                if (!string.IsNullOrEmpty(check)) {
                    Globals.GUI.AddOutput("-Requested patch exists on system");
                    ActivatePatch(check);
                    WaitForCM();
                } else {
                    Globals.GUI.AddOutput("-Requested patch does not exist on system");
                    DownloadPatch();
                    if (!UnpackPatch()) return;
                    HandleSwversion();
                    foreach (var patch in DeactivatedPatches) {
                        var number = patch.Split('-')[1];
                        if (number == Patch) check = patch;
                    }
                    if (check == null) {
                        Globals.GUI.Error("Unexpected error in detecting unpacked patch");
                        return;
                    }
                    ActivatePatch(check);
                    WaitForCM();
                }
            }

            Globals.GUI.AddOutput("");
        }

        // Method for deactivating a patch
        private static bool DeactivatePatch() {
            Globals.GUI.AddOutput($"-Deactivating patch {PatchString}");

            var shellReader = new StreamReader(_shell);
            var shellWriter = new StreamWriter(_shell);

            shellWriter.AutoFlush = true;

            // Flush two lines
            shellWriter.WriteLine();
            shellWriter.WriteLine();

            shellWriter.WriteLine($"/usr/bin/sudo update_deactivate {PatchString}");
            var first = _shell.Expect("will cause a restart. Continue? [Yn]:", new TimeSpan(0, 0, 30));
            if (string.IsNullOrEmpty(first)) {
                Globals.GUI.Error("Failed to start patch activation");
                return false;
            }

            shellWriter.WriteLine("y");

            var second = _shell.Expect("Successfully Deactivated", new TimeSpan(0, 5, 0));
            if (string.IsNullOrEmpty(second)) {
                Globals.GUI.Error("Failed to activate patch");
                return false;
            }

            Globals.GUI.AddOutput($"-Successfully deactivated patch {PatchString}");
            return true;
        }

        // Method for activating a patch
        private static bool ActivatePatch(string patch) {
            Globals.GUI.AddOutput($"-Activating patch {patch}");

            var shellReader = new StreamReader(_shell);
            var shellWriter = new StreamWriter(_shell);

            shellWriter.AutoFlush = true;

            // Flush two lines
            shellWriter.WriteLine();
            shellWriter.WriteLine();

            shellWriter.WriteLine($"/usr/bin/sudo update_activate {patch}");

            var first = _shell.Expect("will cause a restart. Continue? [Yn]:", new TimeSpan(0, 0, 30));
            if (string.IsNullOrEmpty(first)) {
                Globals.GUI.Error("Failed to start patch activation");
                return false;
            }

            shellWriter.WriteLine("y");
            var second = _shell.Expect("Successfully Activated", new TimeSpan(0, 5, 0));
            if (string.IsNullOrEmpty(second)) {
                Globals.GUI.Error("Failed to activate patch");
                return false;
            }

            Globals.GUI.AddOutput($"-Successfully activated patch {patch}");
            return true;
        }

        // Method for downloading the patch
        private static void DownloadPatch() {
            Globals.GUI.AddOutput($"-Downloading patch {Patch}");
            var patch = Globals.CM_PATCHES.Find(a => a.Patch == Patch);
            _lab.Wget(patch.URL, $"/var/home/init/{GetFilename(patch.URL)}");
        }

        // Method for unpacking the patch
        private static bool UnpackPatch() {
            Globals.GUI.AddOutput($"-Unpacking patch {Patch}");
            var patch = Globals.CM_PATCHES.Find(a => a.Patch == Patch);
            var filename = GetFilename(patch.URL);

            var shellReader = new StreamReader(_shell);
            var shellWriter = new StreamWriter(_shell);

            shellWriter.AutoFlush = true;

            // Flush two lines
            shellWriter.WriteLine();
            shellWriter.WriteLine();

            shellWriter.WriteLine($"/usr/bin/sudo update_unpack /var/home/init/{filename}");
            var check = _shell.Expect("unpacked successfully", new TimeSpan(0, 2, 0));
            if (string.IsNullOrEmpty(check)) {
                Globals.GUI.Error("Failed to unpack patch");
                return false;
            }

            Globals.GUI.AddOutput($"-Successfully unpacked patch {filename}");
            return true;
        }

        // Method for waiting for CM to come back up after patch activate/deactivate
        private static bool WaitForCM() {
            Globals.GUI.AddOutput("-Waiting for CM to be in UP state");
            var start = DateTime.Now;
            var done = false;
            var up = false;

            while (!done) {
                Thread.Sleep(15000);
                var statapp = _lab.Statapp();
                var split = statapp.Split('\n');
                foreach (var line in split) {
                    if (!line.Contains("CommunicaMgr")) continue;
                    Globals.GUI.AddStatus(line);
                    var check = line.Split(' ').Where(a => !string.IsNullOrEmpty(a)).ToArray()[1];
                    var fields = check.Split('/');
                    if (fields[0] == fields[1]) {
                        done = true;
                        up = true;
                    } else if ((DateTime.Now - start).TotalMinutes >= 5) {
                        Globals.GUI.Error("Timed out waiting for CM to come up");
                        done = true;
                    }
                }
            }

            return up;
        }

        // Method for setting the MOTD
        private static void SetMOTD() {
            Globals.GUI.AddOutput("-Setting /etc/motd");
            Globals.GUI.AddOutput("");

            var shellReader = new StreamReader(_shell);
            var shellWriter = new StreamWriter(_shell);

            shellWriter.AutoFlush = true;

            // Flush two lines
            shellWriter.WriteLine();
            shellWriter.WriteLine();

            if (!GoSroot(shellWriter)) return;

            var setMOTD = $"/bin/echo 'LAB IN USE - {Globals.USER_DATA.ToolsAUsername.ToUpper()} - {DateTime.Now.ToString("MM/dd")}' > /etc/motd";

            shellWriter.WriteLine(setMOTD);
            Thread.Sleep(2000);
            shellWriter.WriteLine("exit");
        }

        // Method to parse the filename out of a URL
        private static string GetFilename(string url) {
            return url.Split('/').Last();
        }

        // Method for logging in sroot on the shell
        private static bool GoSroot(StreamWriter shellWriter) {
            shellWriter.WriteLine("/bin/su - sroot");
            var first = _shell.Expect(new Regex("Challenge:|Password:"), new TimeSpan(0, 0, 10));
            if (string.IsNullOrEmpty(first)) {
                Globals.GUI.Error("Failed to switch to sroot");
                return false;
            }

            var response = "";
            if (first.Contains("Password:")) {
                response = "sroot01";
            } else {
                var split = first.Split('\n');
                foreach (var line in split) {
                    if (line.Contains("Challenge:")) {
                        response = Connections.WebASGConnection.GetResponse(line, "sroot");
                        //response = Globals.TOOLSA.GetASG(line, "sroot");
                    }
                }
            }

            if (string.IsNullOrEmpty(response)) {
                Globals.GUI.Error("Failed to obtain sroot authentication response");
                return false;
            }

            shellWriter.WriteLine(response);
            return true;
        }
    }
}